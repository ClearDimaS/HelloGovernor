using UnityEngine;

public class WishAssistantItem : MonoBehaviour, IResetable, IKey<ItemsWishGranter>
{
    [field: SerializeField] public ItemsWishGranter Type { get; private set; }
    public ItemsWishGranter Key => Type;
    public bool IsIK { get; set; }

    public void OnReset()
    {
        
    }

    public void OnPool()
    {

    }
}