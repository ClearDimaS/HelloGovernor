using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompasMarker : MonoBehaviour
{
    [field: SerializeField] public RectTransform Root { get; private set; }
    [SerializeField] private Image icon;
    
    public void SetSprite(Sprite sprite)
    {
        icon.sprite = sprite;
    }
}
