using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseHelper 
{
    private static PerlinNoise perlinCache;
    public static float GetPerlinNoise(int x, int y, Vector2 offset, Vector2 scale, int width, int height, bool recreate = false)
    {
        if (perlinCache == null || recreate)
        {
            perlinCache = new PerlinNoise();
        }
        var perlin = perlinCache;
        return perlin.Noise(offset.x + x/(float)width * scale.x, offset.y + y/(float)height * scale.y);
    }
    
    private static ValueNoise valueCache;
    private static Vector2 lastValueSize;
    public static float GetValueNoise(int x, int y, Vector2 offset, Vector2 scale, int gridSize, bool recreate = false)
    {
        if (valueCache == null || recreate || lastValueSize.x != gridSize) ;
        {
            valueCache = new ValueNoise(gridSize);
            lastValueSize = new Vector2(gridSize, gridSize);
        }

        var value = valueCache;
        return value.Noise(offset.x + x * scale.x, offset.y + y * scale.y);
    }

    private static float[,] simplexCache;
    private static Vector2 lastSimplexSize;
    public static float GetSimplexNoise(int x, int y, float scale, int width, int height, bool recreate = false)
    {
        if (simplexCache == null || recreate || lastSimplexSize.x != width || lastSimplexSize.y != height)
        {
            simplexCache = SimplexNoise.Calc2D(width, height, scale);
            lastSimplexSize = new Vector2(width, height);
        }

        var simplex = simplexCache;
        return simplex[x,y];
    }
}
