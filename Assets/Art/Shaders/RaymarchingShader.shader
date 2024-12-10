Shader "FracturedRealm/RaymarchingShader"
{
    HLSLINCLUDE
    
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    // The Blit.hlsl file provides the vertex shader (Vert),
    // the input structure (Attributes), and the output structure (Varyings)
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

    float4 _CamWorldSpace;
    float4x4 _CamFrustum, _CamToWorld;

    struct AttributesCustom
    {
        uint vertexID : SV_VertexID;
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct VaryingsCustom
    {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
        float3 ray : TEXCOORD1;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    VaryingsCustom Vert(AttributesCustom input)
    {
        VaryingsCustom output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

        float4 pos = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.uv = GetFullScreenTriangleTexCoord(input.vertexID);

        output.vertex = pos;

        output.ray = _CamFrustum[(int)input.vertexID].xyz;

        output.ray /= abs(output.ray.z);

        output.ray = mul(_CamToWorld, output.ray);

        return output;
    }

    float4 Frag (VaryingsCustom input) : SV_Target
    {
        //return float4(input.uv.xy, 1, 1);
        float3 rayDir = normalize(input.ray.xyz);
        float3 rayOrigin = _CamWorldSpace.xyz;
        return float4(rayDir, 1);
    }

    ENDHLSL
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "RaymarchingPass"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0
            
            ENDHLSL
        }
    }
}