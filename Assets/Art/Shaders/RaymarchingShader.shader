Shader "FracturedRealm/RaymarchingShader"
{
    HLSLINCLUDE
    
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        // The Blit.hlsl file provides the vertex shader (Vert),
        // the input structure (Attributes), and the output structure (Varyings)
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        #include "Assets\Art\Shaders\RaymarchUtil.hlsl"

        sampler2D _CameraDepthTexture;
        float _Near;
        float _Far;
        float4 _CamWorldPos;
        float4x4 _CamFrustum, _CamToWorld;
        float _MaxDistance;
        float3 _LightDir;

        float4 _ObjectPositions[100];
        float4 _ObjectRotations[100];
        float4 _ObjectScales[100];
        float _ObjectTypes[100];
        int _ObjectCount;

        struct C_Attributes
        {
            uint vertexID : SV_VertexID;
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct C_Varyings
        {
            float4 positionCS : SV_POSITION;
            float2 texcoord : TEXCOORD0;
            float3 ray : TEXCOORD1;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        C_Varyings C_Vert(C_Attributes input)
        {
            C_Varyings output;
            UNITY_SETUP_INSTANCE_ID(input);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
            
            float4 pos = GetQuadVertexPosition(input.vertexID);
            pos.xy -= 0.5f;
            pos.xy *= 2.0f;
            pos.y *= -1.0f;
            float2 uv  = GetQuadTexCoord(input.vertexID);

            output.positionCS = pos;
            output.texcoord   = DYNAMIC_SCALING_APPLY_SCALEBIAS(uv);

            output.ray = _CamFrustum[input.vertexID].xyz;
            output.ray /= abs(output.ray.z);
            output.ray = mul(_CamToWorld, output.ray);

            return output;
        }
     
        float DistanceField(float3 pos)
        {
            // loop trough all objects
            float dist = 0;
            for (int i = 0; i < _ObjectCount; i++)
            {
                float3 p = rotate(pos - _ObjectPositions[i].xyz, _ObjectRotations[i].xyz);
                if (_ObjectTypes[i] == 0)
                {
                    dist += sdMengerSponge(p, average(_ObjectScales[i].xyz), 5);
                }
            }
            // float Sphere1 = sdSphere(pos - float3(0, 0, -2.5), 1.0);
            // float Cube1 = sdCube(rotate(pos - float3(0, 0, 3), float3(0, 1, 0), 45), 0.5);
            // float Cube2 = sdMengerSponge(rotate(pos - float3(0, 0, 3), float3(0, 1, 0), 45), 1.0, 5);
            return dist;
        }

        float3 GetNormal(float3 pos)
        {
            const float2 offset = float2(0.001, 0.0);
            float3 normal = float3(
                DistanceField(pos + offset.xyy) - DistanceField(pos - offset.xyy),
                DistanceField(pos + offset.yxy) - DistanceField(pos - offset.yxy),
                DistanceField(pos + offset.yyx) - DistanceField(pos - offset.yyx)
            );
            return normalize(normal);
        }

        float4 RayMarching(float3 rayOrigin, float3 rayDir, float depth)
        {
            float4 result = float4(1, 1, 1, 1);
            const int maxSteps = 256;
            float distanceTraveled = 0.0; // distance traveled along the ray

            for (int i = 0; i < maxSteps; i++)
            {
                if (distanceTraveled > _MaxDistance || distanceTraveled >= depth)
                {
                    result = float4(rayDir, 0);
                    break;
                }
                float3 pos = rayOrigin + rayDir * distanceTraveled;
                // check for hit
                float distance = DistanceField(pos);
                if (distance < 0.01)
                {
                    float3 normal = GetNormal(pos);
                    float light = dot(-_LightDir, normal);
                    result = float4(float3(1, 1, 1) * light, 1);
                    break;
                }

                distanceTraveled += distance;
            }

            return result;
        }

        float4 Frag (C_Varyings input) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

            float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, input.texcoord).r, _Near, _Far);
            depth *= length(input.ray);

            Varyings inputAlt;
            inputAlt.texcoord = input.texcoord;
            inputAlt.positionCS = input.positionCS;

            float3 oldColor = FragBilinear(inputAlt).rgb;

            float3 rayDir = normalize(input.ray.xyz);
            float3 rayOrigin = _CamWorldPos.xyz;
            float4 color = RayMarching(rayOrigin, rayDir, depth);
            
            return float4(oldColor * (1 - color.a) + color.rgb * color.a, 1);
        }
    
    ENDHLSL
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "PreFrag"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment FragBilinear
            
            ENDHLSL
        }
        
        Pass
        {
            Name "RaymarchingPass"

            HLSLPROGRAM
            
            #pragma vertex C_Vert
            #pragma fragment Frag
            
            ENDHLSL
        }
    }
}