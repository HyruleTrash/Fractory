using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;
using System.Runtime.InteropServices;
using Unity.VisualScripting;

public class RaymarchRendererFeature : ScriptableRendererFeature
{
    public Shader shader;
    private RaymarchPass _raymarchPass;
    public float maxDistance = 100.0f;
    [Range(0.0f, 2.0f)]
    public float lightOffset = 1.0f;

    public override void Create()
    {
        if (shader == null)
            return;

        _raymarchPass = new RaymarchPass(shader, maxDistance, lightOffset);

        _raymarchPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_raymarchPass);
    }

    protected override void Dispose(bool disposing)
    {
        if (Application.isPlaying)
        {
            Destroy(_raymarchPass.raymarchMaterial);
        }
        else
        {
            DestroyImmediate(_raymarchPass.raymarchMaterial);
        }

        if (_raymarchPass.fractalBuffer != null){
            _raymarchPass.fractalBuffer.Release();
            _raymarchPass.fractalBuffer = null;
        }
    }

    public class RaymarchPass : ScriptableRenderPass{
        public float maxDistance;
        public Transform light;
        public float lightOffset = 1.0f;
        public Fractal[] fractals;
        private Shader _shader;
        public ComputeBuffer fractalBuffer;
        private int _fractalBufferSize;
        public Material raymarchMaterial;
        TextureHandle _source, _destination;
        private RenderTextureDescriptor _textureDescriptor;
        private FractalManager _fractalManager;

        public RaymarchPass(Shader shader, float maxDistance, float lightOffset)
        {
            this._shader = shader;
            raymarchMaterial = new Material(shader);
            _textureDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
            this.maxDistance = maxDistance;
            this.lightOffset = lightOffset;
            _fractalBufferSize = Marshal.SizeOf(typeof(Fractal));
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData){
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            _fractalManager = GameObject.FindFirstObjectByType<FractalManager>();
            if (_fractalManager == null)
                return;
            
            if (resourceData.isActiveTargetBackBuffer)
                return;

            // Set the blur texture size to be the same as the camera target size.
            _textureDescriptor.width = cameraData.cameraTargetDescriptor.width;
            _textureDescriptor.height = cameraData.cameraTargetDescriptor.height;
            _textureDescriptor.depthBufferBits = 0;

            _source = resourceData.activeColorTexture;
            _destination = UniversalRenderer.CreateRenderGraphTexture(renderGraph, _textureDescriptor, "RaymarchPass_tex", false);
            
            // This check is to avoid an error from the material preview in the scene
            if (!_source.IsValid() || !_destination.IsValid())
                return;

            fractals = _fractalManager.GetFractals();
            if (fractals.Length == 0)
                return;

            if (!raymarchMaterial){
                raymarchMaterial = new Material(_shader);
                return;
            }
            
            RenderGraphUtils.BlitMaterialParameters output = SetSettings(renderGraph, _destination, _source, cameraData);

            renderGraph.AddBlitPass(output, "RaymarchingPass");

            // if (fractalBuffer != null){
            //     fractalBuffer.Release();
            //     fractalBuffer = null;
            // }
        }

        private RenderGraphUtils.BlitMaterialParameters SetSettings(RenderGraph renderGraph, TextureHandle destination, TextureHandle source, UniversalCameraData cameraData){
            light = GameObject.Find("Directional Light").transform;

            raymarchMaterial.SetMatrix("_CamFrustum", MathUtil.CamFrustum(cameraData.camera));
            raymarchMaterial.SetMatrix("_CamToWorld", cameraData.camera.cameraToWorldMatrix);
            raymarchMaterial.SetVector("_CamForwardOrtho", cameraData.camera.transform.forward);
            raymarchMaterial.SetVector("_CamWorldPos", cameraData.worldSpaceCameraPos);
            raymarchMaterial.SetFloat("_Near", cameraData.camera.nearClipPlane);
            raymarchMaterial.SetFloat("_Far", cameraData.camera.farClipPlane);
            raymarchMaterial.SetFloat("_MaxDistance", maxDistance);
            raymarchMaterial.SetVector("_LightDir", light ? light.forward : Vector3.down);
            raymarchMaterial.SetFloat("_LightOffset", lightOffset);

            if (fractalBuffer == null){
                fractalBuffer = new ComputeBuffer(fractals.Length, _fractalBufferSize, ComputeBufferType.Structured);
                fractalBuffer.name = "Fractal Buffer";
            }
            if (fractals.Length != fractalBuffer.count){
                fractalBuffer.Release();
                fractalBuffer = new ComputeBuffer(fractals.Length, _fractalBufferSize, ComputeBufferType.Structured);
                fractalBuffer.name = "Fractal Buffer";
            }
            raymarchMaterial.SetBuffer("_ObjectsBuffer", fractalBuffer);
            fractalBuffer.SetData(fractals);

            RenderGraphUtils.BlitMaterialParameters prePass = new(source, destination, raymarchMaterial, 0);
            prePass.geometry = RenderGraphUtils.FullScreenGeometryType.ProceduralQuad;
            renderGraph.AddBlitPass(prePass, "RaymarchingPrePass");

            RenderGraphUtils.BlitMaterialParameters output = new(destination, source, raymarchMaterial, 1);
            output.geometry = RenderGraphUtils.FullScreenGeometryType.ProceduralQuad;
            
            return output;
        }
    }
}