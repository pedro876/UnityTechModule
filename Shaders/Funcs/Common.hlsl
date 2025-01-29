#ifndef PIT_COMMON
#define PIT_COMMON

inline float IsTexcoordInside(half2 uv)
{
    uv -= 0.5;
    uv = abs(uv);
    float m = max(uv.x, uv.y);
    return step(m, 0.5);
}

float GetLinearEyeDepth(float rawDepth)
{
    #if defined(ORTHOGRAPHIC)
        return LinearDepthToEyeDepth(rawDepth);
    #else
        return LinearEyeDepth(rawDepth, _ZBufferParams);
    #endif
}

inline float GetRawDepthFromEyeDepth(in float rawDepth)
{
    return 1.0 / (_ZBufferParams.z * rawDepth + _ZBufferParams.w);
}

//Remaps a normalized value to another range
float MinMax(float value, float minVal, float maxVal)
{
    return saturate((value - minVal) / (maxVal - minVal));
}

half3 GetObjectScale()
{
    return half3(
        length(unity_ObjectToWorld._m00_m10_m20),
        length(unity_ObjectToWorld._m01_m11_m21),
        length(unity_ObjectToWorld._m02_m12_m22)
    );
}

#endif