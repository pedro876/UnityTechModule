//https://openprocessing.org/sketch/595507/
//http://kitfox.com/projects/perlinNoiseMaker/
//https://momentsingraphics.de/BlueNoise.html
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace HLSL.NoiseTextures
{
    public class NoiseTextureFeature : ScriptableRendererFeature
    {
        [Header("2D")]
        [SerializeField] Texture2D gradientNoise2D;
        [SerializeField] Texture2D simpleNoise2D;
        [SerializeField] Texture2D voronoiNoise2D;
        [SerializeField] Texture2D blueNoise2D; //LDR_RGBA_9

        static readonly int _NoiseTex_ScreenSize = Shader.PropertyToID(nameof(_NoiseTex_ScreenSize));

        static readonly int _NoiseTex_Gradient2D = Shader.PropertyToID(nameof(_NoiseTex_Gradient2D));
        static readonly int _NoiseTex_Simple2D = Shader.PropertyToID(nameof(_NoiseTex_Simple2D));
        static readonly int _NoiseTex_Voronoi2D = Shader.PropertyToID(nameof(_NoiseTex_Voronoi2D));
        static readonly int _NoiseTex_Blue2D = Shader.PropertyToID(nameof(_NoiseTex_Blue2D));

        public override void Create()
        {
            SetGlobalNoiseTextuers();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            float width = renderingData.cameraData.cameraTargetDescriptor.width;
            float height = renderingData.cameraData.cameraTargetDescriptor.height;
            Shader.SetGlobalVector(_NoiseTex_ScreenSize, new Vector2(width, height));
#if UNITY_EDITOR
            SetGlobalNoiseTextuers();
#endif
        }

        private void SetGlobalNoiseTextuers()
        {
            Shader.SetGlobalTexture(_NoiseTex_Gradient2D, gradientNoise2D);
            Shader.SetGlobalTexture(_NoiseTex_Simple2D, simpleNoise2D);
            Shader.SetGlobalTexture(_NoiseTex_Voronoi2D, voronoiNoise2D);
            Shader.SetGlobalTexture(_NoiseTex_Blue2D, blueNoise2D);
        }
    }
}


