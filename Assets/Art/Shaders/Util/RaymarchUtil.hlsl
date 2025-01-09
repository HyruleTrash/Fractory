#include "Assets\Art\Shaders\Util\GLSLPorted.hlsl"
#include "Assets\Art\Shaders\Util\LengthUtil.hlsl"

float CorrectDepth(float rawDepth, float near, float far)
{
    float persp = LinearEyeDepth(rawDepth, near, far);
    float ortho = (_ProjectionParams.z-_ProjectionParams.y)*(1-rawDepth)+_ProjectionParams.y;
    return lerp(persp,ortho,unity_OrthoParams.w);
}

float LinearEyeDepth(float cameraDepth, float near, float far)
{
    return rcp(_ZBufferParams.z * cameraDepth + _ZBufferParams.w);
}

float opSmoothUnion( float d1, float d2, float k )
{
    float h = clamp( 0.5 + 0.5*(d2-d1)/k, 0.0, 1.0 );
    return lerp( d2, d1, h ) - k*h*(1.0-h);
}

#include "Assets\Art\Shaders\Sdf.hlsl"