Shader "FracturedRealm/RaymarchingShader"
{
    HLSLINCLUDE
    
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    // The Blit.hlsl file provides the vertex shader (Vert),
    // the input structure (Attributes), and the output structure (Varyings)
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

    float4 _CamWorldPos;
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

    float4 GenerateVertex(uint vertexID)
    {
        float4 vertex = float4(0.0f, 0.0f, 0.0f, 1.0f);
        switch(vertexID)
        {
            case 0:
                vertex = float4(0.0f, 0.0f, 3.0f, 1.0f);
                break;
            case 1:
                vertex = float4(1.0f, 0.0f, 2.0f, 1.0f);
                break;
            case 2:
                vertex = float4(1.0f, 1.0f, 1.0f, 1.0f);
                break;
            case 3:
                vertex = float4(0.0f, 1.0f, 0.0f, 1.0f);
                break;
        }

        vertex.xy -= 0.5f;
        vertex.xy *= 2.0f;
        vertex.y *= -1.0f;

        #ifdef UNITY_PRETRANSFORM_TO_DISPLAY_ORIENTATION
            vertex = ApplyPretransformRotation(vertex);
        #endif
        return vertex;
    }

    VaryingsCustom Vert(AttributesCustom input)
    {
        VaryingsCustom output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

        float4 vertex = GenerateVertex(input.vertexID);
        half index = vertex.z;
        input.vertex = 0;
        
        output.vertex = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.vertex.xy = vertex.xy;
        output.uv = GetFullScreenTriangleTexCoord(input.vertexID);

        output.ray = _CamFrustum[(int)index].xyz;
        output.ray /= abs(output.ray.z);
        output.ray = mul(_CamToWorld, output.ray);

        return output;
    }

    float4 Frag (VaryingsCustom input) : SV_Target
    {
        // float3 t = normalize(input.vertex.xyz);
        // return float4(t.z, 1);
        float3 rayDir = normalize(input.ray.xyz);
        float3 rayOrigin = _CamWorldPos.xyz;
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