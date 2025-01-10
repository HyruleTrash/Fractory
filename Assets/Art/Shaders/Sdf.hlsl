float sdSphere(float3 pos, float radius)
{
    return length(pos) - radius;
}

float sdCube(float3 rayPos, float size) {
    float3 b = float3(size, size, size);
    float3 q = abs(rayPos) - b;
    return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
}

float sdCube(float3 rayPos, float3 b) {
    float3 q = abs(rayPos) - b;
    return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
}

float sdRoundCube(float3 p, float size, float r )
{
    float3 b = float3(size, size, size);
    float3 q = abs(p) - b + r;
    return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0) - r;
}

float sdPyramid(float3 p, float size, float h )
{
    p += float3(0,0.5*h,0);
    float m2 = h*h + 0.25;
    
    float usedSize = 1 / size;
    p.xz = abs(p.xz) * usedSize;
    p.xz = (p.z>p.x) ? p.zx : p.xz;
    p.xz -= 0.5;

    float3 q = float3( p.z, h*p.y - 0.5*p.x, h*p.x + 0.5*p.y);
    
    float s = max(-q.x,0.0);
    float t = clamp( (q.y-0.5*p.z)/(m2+0.25), 0.0, 1.0 );
        
    float a = m2*(q.x+s)*(q.x+s) + q.y*q.y;
    float b = m2*(q.x+0.5*t)*(q.x+0.5*t) + (q.y-m2*t)*(q.y-m2*t);
        
    float d2 = min(q.y,-q.x*m2-q.y*0.5) > 0.0 ? 0.0 : min(a,b);
    float sides = sqrt( (d2+q.z*q.z)/m2 ) * sign(max(q.z,-p.y));

    float bottom = sdCube(p + float3(size / 4, 0, size / 4), float3(size / 4, 0.001, size / 4));
        
    return min(sides, bottom);
}

float sdOctahedron( float3 p, float s )
{
    p = abs(p);
    float m = p.x+p.y+p.z-s;
    float3 q;

    if( 3.0*p.x < m ) {
        q = p.xyz;
    }
    else if( 3.0*p.y < m ) {
        q = p.yzx;
    }
    else if( 3.0*p.z < m ) {
        q = p.zxy;
    }
    else {
        return m * 0.57735027;
    }
        
    float k = clamp(0.5*(q.z-q.y+s),0.0,s); 
    return length(float3(q.x,q.y-s+k,q.z-k)); 
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