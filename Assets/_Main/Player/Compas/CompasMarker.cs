using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompasMarker : MonoBehaviour
{
    [field: SerializeField] public RectTransform Root { get; private set; }
    [SerializeField] private Image icon;
    [SerializeField] private Image bg;
    
    public void SetSprite(Sprite sprite)
    {
        icon.sprite = sprite;
    }

    public void SetColor(Color color)
    {
        bg.color = color;
    }
}
