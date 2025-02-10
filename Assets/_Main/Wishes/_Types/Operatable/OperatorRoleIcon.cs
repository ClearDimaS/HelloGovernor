using UnityEngine;

public class OperatorRoleIcon : MonoBehaviour
{
    private const float BASE_SIZE_FACTOR = 0.5f;
    
    [SerializeField] private SpriteRenderer icon;
    private OperatableGranter building;

    private void Awake()
    {
        building = GetComponentInParent<OperatableGranter>(true);

        var sprite = icon.sprite;
        if (building != null)
        {
            sprite = building.GetIconOperator();
        }
        
        icon.sprite = sprite;
        
        var pixelsPerUnit = sprite.rect.width / sprite.bounds.size.x;
        icon.transform.localScale = new Vector3(pixelsPerUnit, pixelsPerUnit, pixelsPerUnit) * BASE_SIZE_FACTOR / sprite.rect.width;  
    }
}