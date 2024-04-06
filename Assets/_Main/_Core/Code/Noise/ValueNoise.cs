using System;

public class ValueNoise
{
    private float[,] grid;
    private int size;

    public ValueNoise(int gridSize)
    {
        size = gridSize;
        grid = new float[size,size];
        var random = new Random();
        for (int i = 0; i < size; i++)
        for (int j = 0; j < size; j++)
            grid[i,j] = (float)random.NextDouble();
    }

    public float Noise(float x, float y)
    {
        int X = (int)x % size;
        int Y = (int)y % size;
        float fractionalX = x - (int)x;
        float fractionalY = y - (int)y;
        
        // Interpolate between grid points
        float v1 = SmoothStep(grid[X % size, Y % size], grid[(X + 1) % size, Y % size], fractionalX);
        float v2 = SmoothStep(grid[X % size, (Y + 1) % size], grid[(X + 1) % size, (Y + 1) % size], fractionalX);
        return SmoothStep(v1, v2, fractionalY);
    }

    private float SmoothStep(float edge0, float edge1, float x)
    {
        // Hermite interpolation
        x = Math.Clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f); 
        return x * x * (3 - 2 * x);
    }
}