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

float3x3 rotation3d(float3 axis, float angle) {
    axis = normalize(axis);
    
    if (axis.x != 0.0) {
        return float3x3(
            1.0, 0.0, 0.0,
            0.0, cos(angle), -sin(angle),
            0.0, sin(angle), cos(angle)
        );
    } else if (axis.y != 0.0) {
        return float3x3(
            cos(angle), 0.0, sin(angle),
            0.0, 1.0, 0.0,
            -sin(angle), 0.0, cos(angle)
        );
    } else {
        return float3x3(
            cos(angle), -sin(angle), 0.0,
            sin(angle), cos(angle), 0.0,
            0.0, 0.0, 1.0
        );
    }
}

float3 rotate(float3 v, float3 axis, float angle) {
    float3x3 m = rotation3d(axis, angle);
    return mul(m, v);
}

float3 rotate(float3 v, float3 r) {
    float3x3 rx = rotation3d(float3(1.0, 0.0, 0.0), 0);
    float3x3 ry = rotation3d(float3(0.0, 1.0, 0.0), 0);
    float3x3 rz = rotation3d(float3(0.0, 0.0, 1.0), 0);
    if (r.x != 0.0){
        rx = rotation3d(float3(r.x, 0.0, 0.0), -r.x);
    }
    if (r.y != 0.0){
        ry = rotation3d(float3(0.0, r.y, 0.0), -r.y);
    }
    if (r.z != 0.0){
        rz = rotation3d(float3(0.0, 0.0, r.z), -r.z);
    }
    float3x3 m = mul(mul(rx, ry), rz);

    return mul(m, v);
}

float sdSphere(float3 pos, float radius)
{
    return length(pos) - radius;
}

float sdCube(float3 rayPos, float size) {
    float3 b = float3(size, size, size);
    float3 q = abs(rayPos) - b;
    return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
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