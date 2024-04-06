using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Texture2DExtensions 
{
    public static void Fill(this Texture2D texture, Color color)
    {
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
    }
}
