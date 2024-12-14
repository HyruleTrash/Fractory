using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class RaymarchRendererFeature : ScriptableRendererFeature
{
    public Shader shader;
    private RaymarchPass _raymarchPass;
    private Material material;
    public float maxDistance = 100.0f;

    public override void Create()
    {
        if (shader == null)
            return;

        material = new Material(shader);
        _raymarchPass = new RaymarchPass(material, maxDistance);

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
            Destroy(material);
        }
        else
        {
            DestroyImmediate(material);
        }
    }


    public class RaymarchPass : ScriptableRenderPass{
        private Camera cam;
        public float maxDistance;
        public Transform light;
        private Material _raymarchMaterial;
        TextureHandle _source, _destination;
        private RenderTextureDescriptor textureDescriptor;

        public RaymarchPass(Material material, float maxDistance)
        {
            _raymarchMaterial = material;
            textureDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
            cam = Camera.main;
            this.maxDistance = maxDistance;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData){
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

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

            RenderGraphUtils.BlitMaterialParameters output = SetSettings(renderGraph, _destination, _source, cameraData);

            renderGraph.AddBlitPass(output, "RaymarchingPass");
        }

        private RenderGraphUtils.BlitMaterialParameters SetSettings(RenderGraph renderGraph, TextureHandle destination, TextureHandle source, UniversalCameraData cameraData){
            if (!_raymarchMaterial)
            {
                return new(destination, source, _raymarchMaterial, 0);
            }

            if (!cam)
            {
                Debug.LogError("Main camera not found");
                return new(destination, source, _raymarchMaterial, 0);
            }
            light = GameObject.Find("Directional Light").transform;

            _raymarchMaterial.SetMatrix("_CamFrustum", CamFrustum(cam));
            _raymarchMaterial.SetVector("_CamWorldPos", cameraData.worldSpaceCameraPos);
            _raymarchMaterial.SetFloat("_MaxDistance", maxDistance);
            _raymarchMaterial.SetVector("_LightDir", light ? light.forward : Vector3.down);

            RenderGraphUtils.BlitMaterialParameters prePass = new(source, destination, _raymarchMaterial, 0);
            prePass.geometry = RenderGraphUtils.FullScreenGeometryType.ProceduralQuad;
            renderGraph.AddBlitPass(prePass, "RaymarchingPrePass");

            RenderGraphUtils.BlitMaterialParameters output = new(destination, source, _raymarchMaterial, 1);
            output.geometry = RenderGraphUtils.FullScreenGeometryType.ProceduralQuad;
            
            return output;
        }

        private Matrix4x4 CamFrustum(Camera cam)
        {
            Matrix4x4 frustum = Matrix4x4.identity;
            float fov = Mathf.Tan((cam.fieldOfView * 0.5f) * Mathf.Deg2Rad);

            Vector3 goUp = Vector3.up * fov;
            Vector3 goRight = Vector3.right * fov * cam.aspect;

            Vector3 TL = -Vector3.forward - goRight + goUp;
            Vector3 TR = -Vector3.forward + goRight + goUp;
            Vector3 BR = -Vector3.forward + goRight - goUp;
            Vector3 BL = -Vector3.forward - goRight - goUp;

            frustum.SetRow(0, TL);
            frustum.SetRow(1, TR);
            frustum.SetRow(2, BR);
            frustum.SetRow(3, BL);

            return frustum;
        }
    }
}