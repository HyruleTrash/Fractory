float length2( float3 p ) { 
    p = p * p;
    return sqrt( p.x+p.y+p.z);
}

// vec2
float length2( float2 p ) { 
    p = p * p;
    return sqrt( p.x+p.y);
}

float length6( float3 p ) {
    p=p*p*p;
    p=p*p;
    return pow(p.x+p.y+p.z,1.0/6.0);
}

float length8( float3 p ) {
    p=p*p;
    p=p*p;
    p=p*p;
    return pow(p.x+p.y+p.z,1.0/8.0);
}

// vec2
float length8( float2 p ) {
    p=p*p;
    p=p*p;
    p=p*p;
    return pow(p.x+p.y,1.0/8.0);
}