using Unity.VisualScripting;
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

    public override void Create()
    {
        if (shader == null)
            return;

        material = new Material(shader);
        _raymarchPass = new RaymarchPass(material);

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
        private Material _raymarchMaterial;
        TextureHandle _source, _destination;
        private RenderTextureDescriptor textureDescriptor;

        public RaymarchPass(Material material)
        {
            _raymarchMaterial = material;
            textureDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
            cam = Camera.main;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData){
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            // Set the blur texture size to be the same as the camera target size.
            textureDescriptor.width = cameraData.cameraTargetDescriptor.width;
            textureDescriptor.height = cameraData.cameraTargetDescriptor.height;
            textureDescriptor.depthBufferBits = 0;

            _source = resourceData.activeColorTexture;
            _destination = UniversalRenderer.CreateRenderGraphTexture(renderGraph, textureDescriptor, "RaymarchPass_tex", false);
            
            RenderGraphUtils.BlitMaterialParameters output = SetSettings(_destination, _source, 0);
            renderGraph.AddBlitPass(output, "RaymarchingPass");
        }

        private RenderGraphUtils.BlitMaterialParameters SetSettings(TextureHandle destination, TextureHandle source, int pass = 0){
            if (!_raymarchMaterial)
            {
                return new(destination, source, _raymarchMaterial, pass);
            }

            if (!cam)
            {
                Debug.LogError("Main camera not found");
                return new(destination, source, _raymarchMaterial, pass);
            }

            _raymarchMaterial.SetMatrix("_CamFrustum", CamFrustum(cam));
            _raymarchMaterial.SetMatrix("_CamToWorld", cam.cameraToWorldMatrix);
            _raymarchMaterial.SetVector("_CamWorldPos", cam.transform.position);

            Debug.Log(cam.cameraToWorldMatrix);

            RenderGraphUtils.BlitMaterialParameters output = new(destination, source, _raymarchMaterial, pass);
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