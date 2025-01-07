using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class RaymarchRendererFeature : ScriptableRendererFeature
{
    public Shader shader;
    private RaymarchPass _raymarchPass;
    public float maxDistance = 100.0f;

    public override void Create()
    {
        if (shader == null)
            return;

        _raymarchPass = new RaymarchPass(shader, maxDistance);

        _raymarchPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_raymarchPass);
    }

    public class RaymarchPass : ScriptableRenderPass{
        public float maxDistance;
        public Transform light;
        public Fractal[] fractals;
        private Shader shader;
        private Material _raymarchMaterial;
        TextureHandle _source, _destination;
        private RenderTextureDescriptor textureDescriptor;
        private FractalManager _fractalManager;

        public RaymarchPass(Shader shader, float maxDistance)
        {
            this.shader = shader;
            _raymarchMaterial = new Material(shader);
            textureDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
            this.maxDistance = maxDistance;
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
            textureDescriptor.width = cameraData.cameraTargetDescriptor.width;
            textureDescriptor.height = cameraData.cameraTargetDescriptor.height;
            textureDescriptor.depthBufferBits = 0;

            _source = resourceData.activeColorTexture;
            _destination = UniversalRenderer.CreateRenderGraphTexture(renderGraph, textureDescriptor, "RaymarchPass_tex", false);
            
            // This check is to avoid an error from the material preview in the scene
            if (!_source.IsValid() || !_destination.IsValid())
                return;

            fractals = _fractalManager.GetFractals();
            if (fractals.Length == 0)
                return;

            if (!_raymarchMaterial){
                _raymarchMaterial = new Material(shader);
                return;
            }
            
            RenderGraphUtils.BlitMaterialParameters output = SetSettings(renderGraph, _destination, _source, cameraData);

            renderGraph.AddBlitPass(output, "RaymarchingPass");
        }

        private RenderGraphUtils.BlitMaterialParameters SetSettings(RenderGraph renderGraph, TextureHandle destination, TextureHandle source, UniversalCameraData cameraData){
            light = GameObject.Find("Directional Light").transform;

            _raymarchMaterial.SetMatrix("_CamFrustum", MathUtil.CamFrustum(cameraData.camera));
            _raymarchMaterial.SetMatrix("_CamToWorld", cameraData.camera.cameraToWorldMatrix);
            _raymarchMaterial.SetVector("_CamForwardOrtho", cameraData.camera.transform.forward * cameraData.camera.farClipPlane);
            _raymarchMaterial.SetVector("_CamWorldPos", cameraData.worldSpaceCameraPos);
            _raymarchMaterial.SetFloat("_Near", cameraData.camera.nearClipPlane);
            _raymarchMaterial.SetFloat("_Far", cameraData.camera.farClipPlane);
            _raymarchMaterial.SetFloat("_MaxDistance", maxDistance);
            _raymarchMaterial.SetVector("_LightDir", light ? light.forward : Vector3.down);

            Vector4[] objectPositions = new Vector4[fractals.Length];
            Matrix4x4[] objectRotations = new Matrix4x4[fractals.Length];
            Vector4[] objectScales = new Vector4[fractals.Length];
            float[] objectTypes = new float[fractals.Length];
            for (int i = 0; i < fractals.Length; i++)
            {
                objectPositions[i] = new Vector4(fractals[i].position.x, fractals[i].position.y, fractals[i].position.z, 0);
                objectRotations[i] = Matrix4x4.TRS(Vector3.zero, fractals[i].rotation, Vector3.one);
                objectScales[i] = new Vector4(fractals[i].scale.x, fractals[i].scale.y, fractals[i].scale.z, 0);
                objectTypes[i] = (float)fractals[i].type;
            }

            _raymarchMaterial.SetVectorArray("_ObjectPositions", objectPositions);
            _raymarchMaterial.SetMatrixArray("_ObjectRotations", objectRotations);
            _raymarchMaterial.SetVectorArray("_ObjectScales", objectScales);
            _raymarchMaterial.SetFloatArray("_ObjectTypes", objectTypes);
            _raymarchMaterial.SetInt("_ObjectCount", fractals.Length);

            RenderGraphUtils.BlitMaterialParameters prePass = new(source, destination, _raymarchMaterial, 0);
            prePass.geometry = RenderGraphUtils.FullScreenGeometryType.ProceduralQuad;
            renderGraph.AddBlitPass(prePass, "RaymarchingPrePass");

            RenderGraphUtils.BlitMaterialParameters output = new(destination, source, _raymarchMaterial, 1);
            output.geometry = RenderGraphUtils.FullScreenGeometryType.ProceduralQuad;
            
            return output;
        }
    }
}