Shader "FracturedRealm/RaymarchingShader"
{
    HLSLINCLUDE
    
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        // The Blit.hlsl file provides the vertex shader (Vert),
        // the input structure (Attributes), and the output structure (Varyings)
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        #include "Assets\Art\Shaders\Util\RaymarchUtil.hlsl"

        sampler2D _CameraDepthTexture;
        float _Near;
        float _Far;
        float4 _CamWorldPos;
        float3 _CamForwardOrtho;
        float4x4 _CamFrustum, _CamToWorld;
        float _MaxDistance;
        float3 _LightDir;
        float _LightOffset;

        struct Object{
            float4 position;
            float4x4 rotation;
            float4 scale;
            float3 color;
            float type;
            float bevel;
            int complexity;
        };
        
        StructuredBuffer<Object> _ObjectsBuffer;

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

            if (unity_OrthoParams.w == 0)
            {// perspective ray casting
                output.ray = _CamFrustum[input.vertexID].xyz;
                output.ray /= abs(output.ray.z);
                output.ray = mul(_CamToWorld, output.ray);
            }
            else
            {// orthographic ray casting
                output.ray = _CamFrustum[input.vertexID].xyz;
                output.ray = mul(_CamToWorld, output.ray);
            }

            return output;
        }
     
        float4 DistanceField(float3 pos)
        {
            // loop trough all objects
            float dist = 0;
            float3 color;
            uint objectCount;
            uint memSize;
            _ObjectsBuffer.GetDimensions(objectCount, memSize);
            for (uint i = 0; i < objectCount; i++)
            {
                float foundDist = 0;
                float3 p = mul(pos - _ObjectsBuffer[i].position.xyz,  _ObjectsBuffer[i].rotation);
                if ( _ObjectsBuffer[i].type == 0)
                {
                    if (_ObjectsBuffer[i].complexity == 0)
                    {
                        foundDist = sdCube(p,  _ObjectsBuffer[i].scale.x / 2, _ObjectsBuffer[i].bevel);
                    }
                    else
                    {
                        float usedScale = average(_ObjectsBuffer[i].scale.xzy);
                        foundDist = sdMengerSponge(p,  float3(1,1,1)*usedScale, _ObjectsBuffer[i].complexity);
                    }
                }
                else if ( _ObjectsBuffer[i].type == 1)
                {
                    foundDist = sdSphere(p, _ObjectsBuffer[i].scale.x / 2);
                }
                else if ( _ObjectsBuffer[i].type == 2)
                {
                    foundDist = sdPyramid(p, average(_ObjectsBuffer[i].scale.xz), _ObjectsBuffer[i].scale.y, _ObjectsBuffer[i].bevel);
                }
                else if ( _ObjectsBuffer[i].type == 3)
                {
                    foundDist = sdOctahedron(p, _ObjectsBuffer[i].scale.x / 2, _ObjectsBuffer[i].bevel);
                }
                
                if (i == 0)
                {
                    dist = foundDist;
                    color = _ObjectsBuffer[i].color;
                }
                else
                {
                    dist = min(dist, foundDist);
                    if (dist == foundDist)
                    {
                        color = _ObjectsBuffer[i].color;
                    }
                }
            }
            if (objectCount == 0)
            {
                return _MaxDistance;
            }
            return float4(color, dist);
        }

        float DistanceFieldOnlyDist(float3 pos){
            return DistanceField(pos).w;
        }

        float3 GetNormal(float3 pos)
        {
            const float2 offset = float2(0.001, 0.0);
            float3 normal = float3(
                DistanceFieldOnlyDist(pos + offset.xyy) - DistanceFieldOnlyDist(pos - offset.xyy),
                DistanceFieldOnlyDist(pos + offset.yxy) - DistanceFieldOnlyDist(pos - offset.yxy),
                DistanceFieldOnlyDist(pos + offset.yyx) - DistanceFieldOnlyDist(pos - offset.yyx)
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
                float4 data = DistanceField(pos);
                float distance = data.w;
                if (distance < 0.0001)
                {
                    float3 normal = GetNormal(pos);
                    float light = dot(-_LightDir, normal);
                    light = max(max(0, light) + _LightOffset, 0);
                    result = float4(data.rgb * light, 1);
                    break;
                }

                distanceTraveled += distance;
            }

            return result;
        }

        float4 Frag (C_Varyings input) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
            
            Varyings inputAlt;
            inputAlt.texcoord = input.texcoord;
            inputAlt.positionCS = input.positionCS;
            
            float4 oldColorFull = FragBilinear(inputAlt).rgba;
            float3 oldColor = oldColorFull.rgb;
            
            float3 rayDir;
            float3 rayOrigin;
            float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, input.texcoord).r, _Near, _Far);
            if (unity_OrthoParams.w == 0)
            {// perspective ray casting
                rayDir = normalize(input.ray.xyz);
                rayOrigin = _CamWorldPos.xyz;
                depth *= length(input.ray);
            }
            else
            {// orthographic ray casting
                rayDir = normalize(_CamForwardOrtho * _Far);
                rayOrigin = input.ray.xyz + _CamWorldPos.xyz;
                depth = CorrectDepth(tex2D(_CameraDepthTexture, input.texcoord).r, _Near, _Far);
            }

            float4 color = RayMarching(rayOrigin, rayDir, depth);
            float3 result = oldColor * (1 - color.a) + color.rgb * color.a;
            if (result.r == oldColor.r && result.g == oldColor.g && result.b == oldColor.b)
            {
                discard;
                return oldColorFull;
            }else{
                return float4(result, 1);
            }
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
            
            #pragma target 5.0
            #pragma vertex Vert
            #pragma fragment FragBilinear
            
            ENDHLSL
        }
        
        Pass
        {
            Name "RaymarchingPass"

            HLSLPROGRAM
            
            #pragma target 5.0
            #pragma vertex C_Vert
            #pragma fragment Frag
            
            ENDHLSL
        }
    }
}