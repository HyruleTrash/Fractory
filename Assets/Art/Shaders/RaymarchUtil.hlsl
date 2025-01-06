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

float average(float3 v)
{
    return (v.x + v.y + v.z) / 3.0;
}

float rcp(float value)
{
    return 1.0 / value;
}

float LinearEyeDepth(float cameraDepth, float near, float far)
{
    return rcp(_ZBufferParams.z * cameraDepth + _ZBufferParams.w);
}

float3 COLOR;

float4x4 rotation3d(float3 axis, float angle) {
    axis = normalize(axis);
    
    float s;
    float c;
    s = sin(angle);
    c = cos(angle);

    float a = modf(angle, (PI / 2) * 6);
    COLOR = float3(a,a,a);

    if (a > 0 && a < PI / 2) {
        // COLOR = float3(0, 1, 0);
        s = sin(angle);
        c = cos(angle);
    }
    else {
        // COLOR = float3(1, 0, 0);
        s = cos(angle);
        c = sin(angle);
    }

    float oc = 1.0 - c;

    return float4x4(
        oc * axis.x * axis.x + c,           oc * axis.x * axis.y - axis.z * s,  oc * axis.z * axis.x + axis.y * s,  0.0,
        oc * axis.x * axis.y + axis.z * s,  oc * axis.y * axis.y + c,           oc * axis.y * axis.z - axis.x * s,  0.0,
        oc * axis.z * axis.x - axis.y * s,  oc * axis.y * axis.z + axis.x * s,  oc * axis.z * axis.z + c,           0.0,
        0.0,                                0.0,                                0.0,                                1.0
    );
}

float3 rotate(float3 v, float3 axis, float angle) {
    float4x4 m = rotation3d(axis, angle);
    return mul(m, float4(v, 1.0)).xyz;
}

float3 rotate(float3 v, float3 r) {
    float4x4 rz = rotation3d(float3(0.0, 0.0, 1.0), 0);
    float4x4 ry = rotation3d(float3(0.0, 1.0, 0.0), 0);
    float4x4 rx = rotation3d(float3(1.0, 0.0, 0.0), 0);
    if (r.z != 0.0){
        rz = rotation3d(float3(0.0, 0.0, r.z), r.z);
    }
    if (r.y != 0.0){
        ry = rotation3d(float3(0.0, r.y, 0.0), r.y);
    }
    if (r.x != 0.0){
        rx = rotation3d(float3(r.x, 0.0, 0.0), r.x);
    }
    float4x4 m = mul(rz, mul(ry, rx));

    return mul(m, float4(v, 1.0)).xyz;
}

float sdSphere(float3 pos, float radius)
{
    return length(pos) - radius;
}

float sdCube(float3 rayPos, float size) {
    float3 b = float3(size, size, size);
    float3 q = abs(rayPos) - b;
    return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
    // const float3 corner = float3(1.0, 1.0, 1.0) * size;
    // float3 ray = abs(rayPos); // fold ray into positive octant
    // float3 cornerToRay = ray - corner;
    // float cornerToRayMaxComponent = max(max(cornerToRay.x, cornerToRay.y), cornerToRay.z);
    // float distToInsideRay = min(cornerToRayMaxComponent, 0.0);
    // float3 closestToOutsideRay = max(cornerToRay, 0.0);

    // return length(closestToOutsideRay) + distToInsideRay;
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
    size /= 2;
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