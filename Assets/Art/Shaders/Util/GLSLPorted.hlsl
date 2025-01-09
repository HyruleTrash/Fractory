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

float average(float2 v)
{
    return (v.x + v.y) / 2.0;
}

float rcp(float value)
{
    return 1.0 / value;
}