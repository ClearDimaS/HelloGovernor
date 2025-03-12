using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRadialFiller : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer spriteRenderer;
    
    private Material fillMaterial;
    private static readonly int Angle = Shader.PropertyToID("_Angle");
    private static readonly int Tint = Shader.PropertyToID("_Color");
    private static readonly int Arc2 = Shader.PropertyToID("_Arc2");
    private static readonly int Arc1 = Shader.PropertyToID("_Arc1");

    public float fillAmount
    {
        get
        {
            return 1f - fillMaterial.GetFloat(Arc2)/360f;
        }
        set
        {
            fillMaterial.SetFloat(Arc2, 360f * (1f - value));
        }
    }
    
    private void Awake()
    {
        fillMaterial = new Material(Shader.Find("Custom/RadialFill"));
        spriteRenderer.material = fillMaterial;
        var col = spriteRenderer.color;
        fillMaterial.SetFloat(Arc1, 0f);
        fillMaterial.SetColor(Tint, new Color(col.r, col.g, col.b, 1f));
        fillMaterial.SetFloat(Angle, 270f);
        spriteRenderer.color = Color.white;
    }
}
