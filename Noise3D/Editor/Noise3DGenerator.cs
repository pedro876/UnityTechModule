#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Noise3DGenerator
{
    public enum Method
    {
        Sphere,
        Perlin,
        Gradient,
        Voronoi,
    }

    const string folder = "Assets\\Scripts\\TechModule\\Noise3D\\";

    private static string FullPath(string name) => folder + name + ".asset";

    private static int s_seed;
    private static float s_cellSize;
    private static int s_cells;
    private static float[,,] s_perlinValues;
    private static Method s_method;
    private static float s_power;
    private static Vector3[,,] s_gradientVectors;

    

    public static Texture3D Generate(string suffix, int resolution, int seed, int cells,
        float edge0, float edge1, float power, bool applySmoothstep, int octaves, bool mipmaps, Method method,
        bool withNormals, float normalsDisplacement, int normalSamples)
    {
        string name = $"Noise3D_{resolution}";
        if (withNormals) name += "_Normals";
        if (suffix.Trim().Length > 0) name += "_" + suffix;

        bool texExists = TryRead(name, out Texture3D tex);
        TextureFormat format = withNormals ? TextureFormat.RGBA32 : TextureFormat.R8;
        if (!texExists || (mipmaps && tex.mipmapCount < 2) || (!mipmaps && tex.mipmapCount >= 2) || tex.format != format)
        {
            tex = new Texture3D(resolution, resolution, resolution, format, mipmaps);
        }

        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Repeat;
        tex.Apply();
        s_seed = seed;
        s_method = method;
        s_power = power;

        List<float[,,]> noiseLayers = new List<float[,,]>();

        int octaveCells = cells;
        for(int i = 0; i < octaves; i++)
        {
            noiseLayers.Add(CreateNoiseGrid(resolution, octaveCells));
            octaveCells *= 2;
        }

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                for (int z = 0; z < resolution; z++)
                {
                    float noiseValue = noiseLayers[0][x, y, z];
                    float totalWeight = 1f;
                    for(int i = 1; i < octaves; i++)
                    {
                        float weight = (1f / (i + 1f));
                        noiseValue += noiseLayers[i][x, y, z] * weight;
                        totalWeight += weight;
                    }

                    noiseValue /= totalWeight;




                    if (method == Method.Perlin || method == Method.Gradient)
                    {
                        noiseValue = Mathf.Clamp(noiseValue, -1f, 1f);
                        noiseValue = noiseValue * 0.5f + 0.5f;
                    }
                    noiseValue = Mathf.Clamp01(noiseValue);
                    noiseValue = Mathf.Pow(noiseValue, s_power);
                    if(applySmoothstep)
                    {
                        noiseValue = Smoothstep(edge0, edge1, noiseValue);
                    }
                    tex.SetPixel(x, y, z, new Color(noiseValue, 0, 0, 0f));
                }
            }
        }

        if (withNormals)
        {
            tex.Apply();
            float fres = (float)resolution;
            Color[,,] newColors = new Color[resolution, resolution, resolution];
            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    for (int z = 0; z < resolution; z++)
                    {
                        float bx = (float)x / fres;
                        float by = (float)y / fres;
                        float bz = (float)z / fres;
                        Vector3 uv = new Vector3(bx, by, bz);
                        float noise = tex.GetPixel(x, y, z).r;
                        Vector3 predominantDirection = Vector3.zero;

                        for (int n = 0; n < normalSamples; n++)
                        {
                            Vector3 displacement = Random.insideUnitSphere * normalsDisplacement;
                            Vector3 otherUV = uv + displacement;
                            float otherNoise = tex.GetPixelBilinear(otherUV.x, otherUV.y, otherUV.z).r;
                            if(otherNoise < noise)
                                predominantDirection += displacement * otherNoise;
                        }

                        if(predominantDirection.magnitude == 0f)
                        {
                            predominantDirection = Vector3.zero;
                        } else predominantDirection = predominantDirection.normalized;

                        predominantDirection *= 0.5f;
                        predominantDirection += (Vector3.one * 0.5f);
                        newColors[x, y, z] = new Color(noise, predominantDirection.x, predominantDirection.y, predominantDirection.z);
                        //newColors[x, y, z] = new Color(predominantDirection.x, predominantDirection.y, predominantDirection.z,1f);

                    }
                }
            }
            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    for (int z = 0; z < resolution; z++)
                    {
                        tex.SetPixel(x, y, z, newColors[x, y, z]);
                    }
                }
            }
        }

        tex.Apply(mipmaps);
        tex.name = name;
        if(texExists)
        {
            EditorUtility.SetDirty(tex);
        }
        else
        {
            Write(tex);
        }

        s_perlinValues = null;
        s_gradientVectors = null;
        Debug.Log("[NOISE 3D] MIP COUNT: " + tex.mipmapCount);
        return tex;
    }

    private static float[,,] CreateNoiseGrid(int resolution, int cells)
    {
        s_cellSize = (float)resolution / (float)cells;
        s_cells = cells;
        switch (s_method)
        {
            case Method.Perlin: GeneratePerlinValues(); break;
            case Method.Gradient: GenerateGradientVectors(); break;
            case Method.Voronoi: GenerateVoronoiPoints(); break;
        }

        float[,,] values = new float[resolution, resolution, resolution];
        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                for (int z = 0; z < resolution; z++)
                {
                    Vector3 coord = new Vector3(x, y, z);
                    //float noiseValue = SamplePerlin3D(coord);
                    float noiseValue = 0.0f;
                    switch (s_method)
                    {
                        case Method.Sphere: noiseValue = SampleSphere(coord, resolution); break;
                        case Method.Perlin: noiseValue = SamplePerlin3D(coord); break;
                        case Method.Gradient: noiseValue = SampleGradient3D(coord); break;
                        case Method.Voronoi: noiseValue = SampleVoronoi3D(coord); break;
                    }
                    //noiseValue = Mathf.Pow(noiseValue, s_power);
                    //noiseValue = Smoothstep(edge0, edge1, noiseValue);
                    values[x, y, z] = noiseValue;
                }
            }
        }
        return values;
    }

    #region IO
    private static void Write(Texture3D texture)
    {
        string path = FullPath(texture.name);
        AssetDatabase.Refresh();

        AssetDatabase.CreateAsset(texture, path);
        EditorApplication.delayCall += AssetDatabase.Refresh;

    }

    private static bool TryRead(string name, out Texture3D tex)
    {
        string path = FullPath(name);
        tex = AssetDatabase.LoadAssetAtPath<Texture3D>(path);
        return tex != null;
    }
    #endregion

    #region COMMON FUNCS

    private static void ToGridCoord(float coord, out float frac, out int lowerIdx, out int higherIdx, out int unboundLowerIdx, out int unboundHigherIdx, bool smoothFrac = true)
    {
        float x = (coord / s_cellSize) - 0.5f;

        frac = x - Mathf.Floor(x);
        if(smoothFrac) frac = Smoothstep(0f, 1f, frac);
        lowerIdx = Mathf.FloorToInt(x);
        higherIdx = lowerIdx + 1;
        unboundLowerIdx = lowerIdx;
        unboundHigherIdx = higherIdx;
        lowerIdx = RepeatGridCoord(lowerIdx);
        higherIdx = RepeatGridCoord(higherIdx);
        //if (lowerIdx < 0) lowerIdx = s_cells - 1;
        //if (higherIdx < 0) higherIdx = s_cells - 1;
        //if (lowerIdx >= s_cells) lowerIdx = 0;
        //if (higherIdx >= s_cells) higherIdx = 0;
    }

    private static (int x, int y, int z) RepeatGridCoord(int x, int y, int z)
    {
        return (RepeatGridCoord(x), RepeatGridCoord(y), RepeatGridCoord(z));
    }

    private static int RepeatGridCoord(int x)
    {
        if (x < 0) x = s_cells - 1;
        if (x >= s_cells) x = 0;
        return x;
    }

    private static float Smoothstep(float edge0, float edge1, float t)
    {
        t = Mathf.Clamp01((t - edge0) / (edge1 - edge0));
        return t * t * (3.0f - 2.0f * t);
    }
    #endregion

    #region SPHERE

    private static float SampleSphere(Vector3 coord, int resolution)
    {
        float noiseValue;
        float bx = coord.x / resolution;
        float by = coord.y / resolution;
        float bz = coord.z / resolution;
        Vector3 uv = new Vector3(bx - 0.5f, by - 0.5f, bz - 0.5f);
        float dist = uv.magnitude;
        if (dist > 0.5f) noiseValue = 0f;
        else noiseValue = 1f;//1f - Mathf.SmoothStep(0, 1, dist / 0.5f);
        return noiseValue;
    }

    #endregion

    #region PERLIN

    private static void GeneratePerlinValues()
    {
        Random.InitState(s_seed);

        s_perlinValues = new float[s_cells, s_cells, s_cells];

        for (int x = 0; x < s_cells; x++)
        {
            for (int y = 0; y < s_cells; y++)
            {
                for (int z = 0; z < s_cells; z++)
                {
                    s_perlinValues[x, y, z] = Mathf.Clamp01(Random.Range(-1f,1f));

                }
            }
        }
    }

    private static float SamplePerlin3D(Vector3 coord)
    {
        ToGridCoord(coord.z, out float fracZ, out int lowerZ, out int higherZ, out int unboundZ0, out int unboundZ1);
        float lowerValue = SamplePerlin2D(coord.x, coord.y, lowerZ);
        float higherValue = SamplePerlin2D(coord.x, coord.y, higherZ);
        float value = Mathf.Lerp(lowerValue, higherValue, fracZ);
        return value;
    }

    private static float SamplePerlin2D(float coordx, float coordy, int z)
    {
        ToGridCoord(coordy, out float fracY, out int lowerY, out int higherY, out int unboundY0, out int unboundY1);
        float lowerValue = SamplePerlin1D(coordx, lowerY, z);
        float higherValue = SamplePerlin1D(coordx, higherY, z);
        float value = Mathf.Lerp(lowerValue, higherValue, fracY);
        return value;

    }

    private static float SamplePerlin1D(float coordx, int y, int z)
    {
        ToGridCoord(coordx, out float frac, out int lowerIdx, out int higherIdx, out int unboundX0, out int unboundX1);
        float lowerValue = s_perlinValues[lowerIdx, y, z];
        float higherValue = s_perlinValues[higherIdx, y, z];
        float value = Mathf.Lerp(lowerValue, higherValue, frac);
        return value;
    }
    #endregion

    #region GRADIENT

    private static void GenerateGradientVectors()
    {
        Random.InitState(s_seed);

        s_gradientVectors = new Vector3[s_cells, s_cells, s_cells];

        for (int x = 0; x < s_cells; x++)
        {
            for (int y = 0; y < s_cells; y++)
            {
                for (int z = 0; z < s_cells; z++)
                {
                    s_gradientVectors[x, y, z] = Random.onUnitSphere;
                }
            }
        }
    }

    private static float SampleGradient3D(Vector3 coord)
    {
        ToGridCoord(coord.x, out float fracX, out int x0, out int x1, out int unboundX0, out int unboundX1);
        ToGridCoord(coord.y, out float fracY, out int y0, out int y1, out int unboundY0, out int unboundY1);
        ToGridCoord(coord.z, out float fracZ, out int z0, out int z1, out int unboundZ0, out int unboundZ1);

        Vector3 gradient_000 = s_gradientVectors[x0, y0, z0];
        Vector3 gradient_001 = s_gradientVectors[x0, y0, z1];
        Vector3 gradient_010 = s_gradientVectors[x0, y1, z0];
        Vector3 gradient_011 = s_gradientVectors[x0, y1, z1];
        Vector3 gradient_100 = s_gradientVectors[x1, y0, z0];
        Vector3 gradient_101 = s_gradientVectors[x1, y0, z1];
        Vector3 gradient_110 = s_gradientVectors[x1, y1, z0];
        Vector3 gradient_111 = s_gradientVectors[x1, y1, z1];

        Vector3 corner_000 = new Vector3(unboundX0, unboundY0, unboundZ0);
        Vector3 corner_001 = new Vector3(unboundX0, unboundY0, unboundZ1);
        Vector3 corner_010 = new Vector3(unboundX0, unboundY1, unboundZ0);
        Vector3 corner_011 = new Vector3(unboundX0, unboundY1, unboundZ1);
        Vector3 corner_100 = new Vector3(unboundX1, unboundY0, unboundZ0);
        Vector3 corner_101 = new Vector3(unboundX1, unboundY0, unboundZ1);
        Vector3 corner_110 = new Vector3(unboundX1, unboundY1, unboundZ0);
        Vector3 corner_111 = new Vector3(unboundX1, unboundY1, unboundZ1);

        Vector3 cellCoord = coord / s_cellSize;

        Vector3 dir_000 = corner_000 - cellCoord;
        Vector3 dir_001 = corner_001 - cellCoord;
        Vector3 dir_010 = corner_010 - cellCoord;
        Vector3 dir_011 = corner_011 - cellCoord;
        Vector3 dir_100 = corner_100 - cellCoord;
        Vector3 dir_101 = corner_101 - cellCoord;
        Vector3 dir_110 = corner_110 - cellCoord;
        Vector3 dir_111 = corner_111 - cellCoord;

        float dot_000 = Vector3.Dot(dir_000, gradient_000);
        float dot_001 = Vector3.Dot(dir_001, gradient_001);
        float dot_010 = Vector3.Dot(dir_010, gradient_010);
        float dot_011 = Vector3.Dot(dir_011, gradient_011);
        float dot_100 = Vector3.Dot(dir_100, gradient_100);
        float dot_101 = Vector3.Dot(dir_101, gradient_101);
        float dot_110 = Vector3.Dot(dir_110, gradient_110);
        float dot_111 = Vector3.Dot(dir_111, gradient_111);

        float bottom = Mathf.Lerp(Mathf.Lerp(dot_000, dot_100, fracX), Mathf.Lerp(dot_001, dot_101, fracX), fracZ);
        float top = Mathf.Lerp(Mathf.Lerp(dot_010, dot_110, fracX), Mathf.Lerp(dot_011, dot_111, fracX), fracZ);
        float middle = Mathf.Lerp(bottom, top, fracY);

        float norm = Mathf.Clamp(middle, -2f, 2f) * 0.5f;

        return Mathf.Clamp(norm, -1f,1f);
    }
    #endregion

    #region VORONOI

    private static void GenerateVoronoiPoints()
    {
        Random.InitState(s_seed);

        s_gradientVectors = new Vector3[s_cells, s_cells, s_cells];

        for (int x = 0; x < s_cells; x++)
        {
            for (int y = 0; y < s_cells; y++)
            {
                for (int z = 0; z < s_cells; z++)
                {
                    s_gradientVectors[x, y, z] = new Vector3(Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f));
                }
            }
        }
    }

    private static float SampleVoronoi3D(Vector3 coord)
    {
        ToGridCoord(coord.x, out float fracX, out int x0, out int x1, out int unboundX0, out int unboundX1, smoothFrac:false);
        ToGridCoord(coord.y, out float fracY, out int y0, out int y1, out int unboundY0, out int unboundY1, smoothFrac:false);
        ToGridCoord(coord.z, out float fracZ, out int z0, out int z1, out int unboundZ0, out int unboundZ1, smoothFrac:false);

        //fracX = coord.x / s_cellSize;
        //fracX -= Mathf.Floor(fracX);
        //fracY = coord.y / s_cellSize;
        //fracY -= Mathf.Floor(fracY);
        //fracZ = coord.z / s_cellSize;
        //fracZ -= Mathf.Floor(fracZ);

        Vector3 fracCoord = new Vector3(fracX, fracY, fracZ);
        float minDistance = float.MaxValue;// Vector3.Distance(fracCoord, s_gradientVectors[x0, y0, z0]);

        for(int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    minDistance = Mathf.Min(minDistance, GetVoronoiDistanceAtGridCoord(fracCoord, x0, y0, z0, dx, dy, dz));
                }
            }
        }

        //minDistance = GetVoronoiDistanceAtGridCoord(fracCoord, x0, y0, z0, 0,0,0);


        return Mathf.Clamp01(minDistance);

        float GetVoronoiDistanceAtGridCoord(Vector3 fracCoord, int x, int y, int z, int dx, int dy, int dz)
        {
            (x, y, z) = RepeatGridCoord(x + dx, y + dy, z + dz);
            return Vector3.Distance(fracCoord, s_gradientVectors[x, y, z] + new Vector3(dx,dy,dz));
        }
    }

    #endregion
}
#endif