using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleIcon : MonoBehaviour
{
    private const float BASE_SIZE_FACTOR = 0.5f;
    
    [SerializeField] private SpriteRenderer icon;
    private UpgradableBuilding building;

    private void Awake()
    {
        building = GetComponentInParent<UpgradableBuilding>(true);

        var sprite = icon.sprite;
        if (building != null)
        {
            sprite = building.GetIconAssistant();
        }
        
        icon.sprite = sprite;
        
        var pixelsPerUnit = sprite.rect.width / sprite.bounds.size.x;
        icon.transform.localScale = new Vector3(pixelsPerUnit, pixelsPerUnit, pixelsPerUnit) * BASE_SIZE_FACTOR / sprite.rect.width;  
    }
}
