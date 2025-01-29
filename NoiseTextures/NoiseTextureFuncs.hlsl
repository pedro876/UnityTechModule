#ifndef NOISE_TEXTURE_FUNCS
#define NOISE_TEXTURE_FUNCS

float2 _NoiseTex_ScreenSize;
SamplerState noise_linear_repeat_sampler;

Texture2D<float> _NoiseTex_Gradient2D;
Texture2D<float> _NoiseTex_Simple2D;
Texture2D<float> _NoiseTex_Voronoi2D;
Texture2D<float> _NoiseTex_Blue2D;
float4 _NoiseTex_Blue2D_TexelSize;

#define GRADIENT_COMPENSATION 0.018
#define SIMPLE_COMPENSATION 0.005
#define VORONOI_COMPENSATION 0.08
#define BLUE_COMPENSATION 0.08

#define SAMPLE_NOISE(tex, compensation) (tex.Sample(noise_linear_repeat_sampler, UV * Scale * compensation).r)
//#define PROCESS_NOISE(noise) (smoothstep(0.0,0.9,noise))
#define PROCESS_NOISE(noise) (noise)

void SampleGradient2D_float(float2 UV, float Scale, out float Out)
{
    Out = PROCESS_NOISE(SAMPLE_NOISE(_NoiseTex_Gradient2D, GRADIENT_COMPENSATION));
}

void SampleSimple2D_float(float2 UV, float Scale, out float Out)
{
    Out = PROCESS_NOISE(SAMPLE_NOISE(_NoiseTex_Simple2D, SIMPLE_COMPENSATION));
}

void SampleVoronoi2D_float(float2 UV, float Scale, out float Out)
{
    Out = PROCESS_NOISE(SAMPLE_NOISE(_NoiseTex_Voronoi2D, VORONOI_COMPENSATION));
}

void SampleBlue2D_float(float2 UV, out float4 Out)
{
    #if SHADERGRAPH_PREVIEW
    Out = _NoiseTex_Blue2D.Load(int3(UV * _NoiseTex_Blue2D_TexelSize.zw,0));
    #else
    UV *= 0.5;
    uint width = _NoiseTex_Blue2D_TexelSize.z;
    uint height = _NoiseTex_Blue2D_TexelSize.w;
    uint x = (UV.x * _NoiseTex_ScreenSize.x);
    uint y = (UV.y * _NoiseTex_ScreenSize.y);
    x %= width;
    y %= height;
    Out = _NoiseTex_Blue2D.Load(int3(x,y,0));
    #endif
}

#undef SAMPLE_NOISE
#define SAMPLE_NOISE(tex, compensation) (tex.SampleLevel(noise_linear_repeat_sampler, UV * Scale * compensation, Lod).r)

void SampleGradient2DLod_float(float2 UV, float Scale, float Lod, out float Out)
{
    Out = PROCESS_NOISE(SAMPLE_NOISE(_NoiseTex_Gradient2D, GRADIENT_COMPENSATION));
}

void SampleSimple2DLod_float(float2 UV, float Scale, float Lod, out float Out)
{
    Out = PROCESS_NOISE(SAMPLE_NOISE(_NoiseTex_Simple2D, SIMPLE_COMPENSATION));
}

void SampleVoronoi2DLod_float(float2 UV, float Scale, float Lod, out float Out)
{
    Out = PROCESS_NOISE(SAMPLE_NOISE(_NoiseTex_Voronoi2D, VORONOI_COMPENSATION));
}

static const float bayerMatrix[] =
{
    00.0 / 16.0, 12.0 / 16.0, 03.0 / 16.0, 15.0 / 16.0,
    08.0 / 16.0, 04.0 / 16.0, 11.0 / 16.0, 07.0 / 16.0,
    02.0 / 16.0, 14.0 / 16.0, 01.0 / 16.0, 13.0 / 16.0,
    10.0 / 16.0, 06.0 / 16.0, 09.0 / 16.0, 05.0 / 16.0
};

static half ditherMatrix[16] =
{
    1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
    13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
    4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
    16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
};

inline uint Get4x4Idx(uint x, uint y)
{
    x = x % 4;
    y = y % 4;
    return y * 4 + x;
}

inline uint Get4x4Idx(float2 uv)
{
    uv *= _NoiseTex_ScreenSize;
    uint x = uv.x;
    uint y = uv.y;
    return Get4x4Idx(x, y);
}

inline uint Get4x4Idx(float2 uv, float renderScale)
{
    uv *= _NoiseTex_ScreenSize * renderScale;
    uint x = uv.x;
    uint y = uv.y;
    return Get4x4Idx(x, y);
}

inline float SampleBayerNoise(uint idx)
{
    return bayerMatrix[idx];
}

inline float SampleBayerNoise(uint x, uint y)
{
    return bayerMatrix[Get4x4Idx(x, y)];
}

inline float SampleBayerNoise(float2 uv)
{
    return bayerMatrix[Get4x4Idx(uv)];
}

inline float SampleDitherNoise(uint idx)
{
    return ditherMatrix[idx];
}

inline float SampleDitherNoise(uint x, uint y)
{
    return ditherMatrix[Get4x4Idx(x, y)];
}

inline float SampleDitherNoise(float2 uv)
{
    return ditherMatrix[Get4x4Idx(uv)];
}

void SampleDitherNoise_float(float2 uv, out float dither)
{
    dither = ditherMatrix[Get4x4Idx(uv)];
}

void DitheredTransparency_float(float2 uv, float opacity01, out float transparency)
{
    UNITY_BRANCH
    if(opacity01 >= 1.0)
    {
        transparency = 1.0;
    }
    else
    {
        float ditherInput = opacity01 * 2.0;
        float dither = ditherMatrix[Get4x4Idx(uv)];
        transparency = saturate(ditherInput - dither);
    }
}

void JitterNormalPerPixel_half(half2 ScreenUV, half3 Normal, half Strength, out half3 JitteredNormal)
{
	float noise = SampleBayerNoise(ScreenUV) - 0.5;
    noise *= (Strength+0.1);
    noise *= 0.03;
	JitteredNormal = Normal + half3(noise,noise,noise);
}

#endif