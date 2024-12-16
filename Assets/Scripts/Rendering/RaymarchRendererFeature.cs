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
        public float maxDistance;
        public Transform light;
        private Material _raymarchMaterial;
        TextureHandle _source, _destination;
        private RenderTextureDescriptor textureDescriptor;
        private FractalManager _fractalManager;

        public RaymarchPass(Material material, float maxDistance)
        {
            _raymarchMaterial = material;
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

            RenderGraphUtils.BlitMaterialParameters output = SetSettings(renderGraph, _destination, _source, cameraData);

            renderGraph.AddBlitPass(output, "RaymarchingPass");
        }

        private RenderGraphUtils.BlitMaterialParameters SetSettings(RenderGraph renderGraph, TextureHandle destination, TextureHandle source, UniversalCameraData cameraData){
            if (!_raymarchMaterial)
            {
                return new(destination, source, _raymarchMaterial, 0);
            }
            light = GameObject.Find("Directional Light").transform;

            _raymarchMaterial.SetMatrix("_CamFrustum", CamFrustum(cameraData.camera));
            _raymarchMaterial.SetMatrix("_CamToWorld", cameraData.camera.cameraToWorldMatrix);
            _raymarchMaterial.SetVector("_CamWorldPos", cameraData.worldSpaceCameraPos);
            _raymarchMaterial.SetFloat("_Near", cameraData.camera.nearClipPlane);
            _raymarchMaterial.SetFloat("_Far", cameraData.camera.farClipPlane);
            _raymarchMaterial.SetFloat("_MaxDistance", maxDistance);
            _raymarchMaterial.SetVector("_LightDir", light ? light.forward : Vector3.down);

            Fractal[] fractals = _fractalManager.GetFractals();
            Vector4[] objectPositions = new Vector4[fractals.Length];
            Vector4[] objectRotations = new Vector4[fractals.Length];
            Vector4[] objectScales = new Vector4[fractals.Length];
            float[] objectTypes = new float[fractals.Length];
            for (int i = 0; i < fractals.Length; i++)
            {
                objectPositions[i] = new Vector4(fractals[i].position.x, fractals[i].position.y, fractals[i].position.z, 0);
                objectRotations[i] = new Vector4(fractals[i].rotation.x * Mathf.Deg2Rad, fractals[i].rotation.y * Mathf.Deg2Rad, fractals[i].rotation.z * Mathf.Deg2Rad, 0);
                objectScales[i] = new Vector4(fractals[i].scale.x, fractals[i].scale.y, fractals[i].scale.z, 0);
                objectTypes[i] = (float)fractals[i].type;

                Debug.Log("Fractal " + i + " rotation: " + objectRotations[i]);
            }

            _raymarchMaterial.SetVectorArray("_ObjectPositions", objectPositions);
            _raymarchMaterial.SetVectorArray("_ObjectRotations", objectRotations);
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

        private Matrix4x4 CamFrustum(Camera cam)
        {
            Matrix4x4 frustum = Matrix4x4.identity;

            if (cam.orthographic)
            {
                Vector3 goUp = Vector3.up * cam.orthographicSize;
                Vector3 goRight = Vector3.right * cam.orthographicSize;

                Vector3 TL = -Vector3.forward - goRight + goUp;
                Vector3 TR = -Vector3.forward + goRight + goUp;
                Vector3 BR = -Vector3.forward + goRight - goUp;
                Vector3 BL = -Vector3.forward - goRight - goUp;

                frustum.SetRow(0, TL);
                frustum.SetRow(1, BL);
                frustum.SetRow(2, BR);
                frustum.SetRow(3, TR);
            }else{
                float fov = Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

                Vector3 goUp = Vector3.up * fov;
                Vector3 goRight = Vector3.right * fov * cam.aspect;

                Vector3 TL = -Vector3.forward - goRight + goUp;
                Vector3 TR = -Vector3.forward + goRight + goUp;
                Vector3 BR = -Vector3.forward + goRight - goUp;
                Vector3 BL = -Vector3.forward - goRight - goUp;

                frustum.SetRow(0, TL);
                frustum.SetRow(1, BL);
                frustum.SetRow(2, BR);
                frustum.SetRow(3, TR);
            }

            return frustum;
        }
    }
}