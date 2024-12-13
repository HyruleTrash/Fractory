Shader "FracturedRealm/RaymarchingShader"
{
    HLSLINCLUDE
    
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    // The Blit.hlsl file provides the vertex shader (Vert),
    // the input structure (Attributes), and the output structure (Varyings)
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

    float4 _CamWorldPos;
    float4x4 _CamFrustum;
    float _MaxDistance;
    float3 _LightDir;

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
    
    float4x4 InverseZAxisCamToWorld(float4x4 camToWorldMatrix)
    {
        // Create a copy of the input matrix
        float4x4 modifiedMatrix = camToWorldMatrix;
    
        // Negate the z-component of the translation
        modifiedMatrix[3][2] = -modifiedMatrix[3][2];
    
        return modifiedMatrix;
    }

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
        float4x4 _CamToWorld = InverseZAxisCamToWorld(unity_CameraToWorld);
        output.ray = mul(_CamToWorld, output.ray);

        return output;
    }

    float fract(float x)
    {
        return x - floor(x);
    }

    float modf(float x, float y)
    {
        return x - y * floor(x / y);
    }

    float3 mod(float3 x, float y) {
        return float3(
            modf(x.x, y),
            modf(x.y, y), 
            modf(x.z, y)
        );
    }

    float sdSphere(float3 pos, float radius)
    {
        return length(pos) - radius;
    }

    float sdCube(float3 rayPos, float size) {
        const float3 corner = float3(1.0, 1.0, 1.0) * size;
        float3 ray = abs(rayPos); // fold ray into positive octant
        float3 cornerToRay = ray - corner;
        float cornerToRayMaxComponent = max(max(cornerToRay.x, cornerToRay.y), cornerToRay.z);
        float distToInsideRay = min(cornerToRayMaxComponent, 0.0);
        float3 closestToOutsideRay = max(cornerToRay, 0.0);
        return length(closestToOutsideRay) + distToInsideRay;
    } 

    float sdCross(float3 rayPos, float size) {
        const float3 corner = float3(1.0, 1.0, 1.0) * size;
        float3 ray = abs(rayPos); // fold ray into positive quadrant
        float3 cornerToRay = ray - corner;
    
        float smallestComp = min(min(cornerToRay.x, cornerToRay.y), cornerToRay.z);
        float largestComp = max(max(cornerToRay.x, cornerToRay.y), cornerToRay.z);
        float middleComp = cornerToRay.x + cornerToRay.y + cornerToRay.z
                                - smallestComp - largestComp;
                
        float2 closestOutsidePoint = max(float2(smallestComp, middleComp), 0.0);
        float2 closestInsidePoint = min(float2(middleComp, largestComp), 0.0);
    
        return length(closestOutsidePoint) + -length(closestInsidePoint);
    }

    float sdMengerSponge(float3 rayPos, float size, int numIterations) {
        const float cubeWidth = size * 2.0;
        const float oneThird = 1.0 / 3.0;
        float spongeCube = sdCube(rayPos, size);
        float mengerSpongeDist = spongeCube;
        
        float scale = 1.0;
        for(int i = 0; i < numIterations; ++i) {
            // #1 determine repeated box width
            float boxedWidth = cubeWidth / scale;
            
            float translation = -boxedWidth / 2.0;
            float3 ray = rayPos - translation;
            float3 repeatedPos = mod(ray, boxedWidth);
            repeatedPos += translation;
            
            // #2 scale coordinate systems from 
            // [-1/scale, 1/scale) -> to [-1.0, 1.0)
            repeatedPos *= scale; 
            
            float crossesDist = sdCross(repeatedPos / oneThird, size) * oneThird;
            
            // #3 Acquire actual distance by un-stretching
            crossesDist /= scale;
            
            mengerSpongeDist = max(mengerSpongeDist, -crossesDist);
            
            scale *= 3.0;
        }
        return mengerSpongeDist;
    }

    float DistanceField(float3 pos)
    {
        float Sphere1 = sdSphere(pos - float3(0, 0, -2.5), 1.0);
        //float Cube1 = sdSierpinskiCarpetCube(pos - float3(0, 1, -3), 1, 5);
        float Cube2 = sdMengerSponge(pos - float3(0, 0, -3), 1.0, 5);
        return Cube2;
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

    float4 RayMarching(float3 rayOrigin, float3 rayDir){
        float4 result = float4(1, 1, 1, 1);
        const int maxSteps = 256;
        float distanceTraveled = 0.0; // distance traveled along the ray

        for (int i = 0; i < maxSteps; i++)
        {
            if (distanceTraveled > _MaxDistance)
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
                result = float4(1, 1, 1, 1) * light;
                //result = float4(normal, 1);
                break;
            }

            distanceTraveled += distance;
        }

        return result;
    }

    float4 Frag (VaryingsCustom input) : SV_Target
    {
        float3 rayDir = normalize(input.ray.xyz);
        float3 rayOrigin = _CamWorldPos.xyz;
        float4 color = RayMarching(rayOrigin, rayDir);
        return color;
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
            
            ENDHLSL
        }
    }
}