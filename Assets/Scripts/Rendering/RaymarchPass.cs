using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule.Util;

public class RaymarchPass : ScriptableRenderPass
{
    private Material _raymarchMaterial;
    private Camera _camera;

    public RaymarchPass(Material material)
    {
        _raymarchMaterial = material;
    }

    public void Setup(Camera camera)
    {
        _camera = camera;
    }

    // Use the RecordRenderGraph method to configure the input and output parameters for the AddBlitPass method and execute the AddBlitPass method.
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if (_raymarchMaterial == null || _camera == null)
            return;
        
        var resourceData = frameData.Get<UniversalResourceData>();

        var source = resourceData.activeColorTexture;

        // Define the texture descriptor for creating the destination render graph texture.
        var destinationDesc = renderGraph.GetTextureDesc(source);
        destinationDesc.name = $"CameraColor-{typeof(RaymarchPass).Name}";
        destinationDesc.clearBuffer = false;
        destinationDesc.depthBufferBits = 0;

        // Create the texture.
        TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

        _raymarchMaterial.SetMatrix("_CamFrustum", CamFrustum(_camera));
        _raymarchMaterial.SetMatrix("_CamToWorld", _camera.cameraToWorldMatrix);
        _raymarchMaterial.SetVector("_CamWorldPos", _camera.transform.position);

        GL.PushMatrix();
        GL.LoadOrtho();
        _raymarchMaterial.SetPass(0);
        GL.Begin(GL.QUADS);

        // Bottom left
        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.Vertex3(0.0f, 0.0f, 3.0f);
        // Bottom right
        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 2.0f);
        // Top right
        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 1.0f);
        // Top left
        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();
        GL.PopMatrix();

        RenderGraphUtils.BlitMaterialParameters para = new(source, destination, _raymarchMaterial, 0);
        renderGraph.AddBlitPass(para, passName: typeof(RaymarchPass).Name);

        resourceData.cameraColor = destination;
    }

    private Matrix4x4 CamFrustum(Camera cam)
    {
        Matrix4x4 frustum = Matrix4x4.identity;
        float fov = Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

        Vector3 goUp = Vector3.up * fov;
        Vector3 goRight = Vector3.right * fov * cam.aspect;

        Vector3 topLeft = -Vector3.forward - goRight + goUp;
        Vector3 topRight = -Vector3.forward + goRight + goUp;
        Vector3 bottomRight = -Vector3.forward + goRight - goUp;
        Vector3 bottomLeft = -Vector3.forward - goRight - goUp;

        frustum.SetRow(0, topLeft);
        frustum.SetRow(1, topRight);
        frustum.SetRow(2, bottomRight);
        frustum.SetRow(3, bottomLeft);

        return frustum;
    }
}