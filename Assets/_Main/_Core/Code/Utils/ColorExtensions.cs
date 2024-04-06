using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtensions 
{
    public static float Distance(this Color c1, Color c2)
    {
        return Mathf.Abs(c1.r - c2.r) + Mathf.Abs(c1.g - c2.g) + Mathf.Abs(c1.b - c2.b);
    }

    public static Color32 ToColor32(this Color c)
    {
        return c;
    }
    
    public static int Distance(this Color32 c1, Color32 c2)
    {
        return Mathf.Abs(c1.r - c2.r) + Mathf.Abs(c1.g - c2.g) + Mathf.Abs(c1.b - c2.b);
    }
}
