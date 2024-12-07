using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RaymarchRendererFeature : ScriptableRendererFeature
{
    public Shader shader;
    private Material _raymarchMaterial;
    private RaymarchPass _raymarchPass;

    public override void Create()
    {
        if (shader == null)
            return;

        _raymarchMaterial = CoreUtils.CreateEngineMaterial(shader);
        _raymarchPass = new RaymarchPass(_raymarchMaterial)
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_raymarchMaterial == null)
            return;

        _raymarchPass.Setup(renderingData.cameraData.camera);
        renderer.EnqueuePass(_raymarchPass);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(_raymarchMaterial);
    }
}