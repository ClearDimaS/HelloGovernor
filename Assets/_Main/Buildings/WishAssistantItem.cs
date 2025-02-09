using UnityEngine;

public class WishAssistantItem : MonoBehaviour, IResetable, IKey<WishGranter>
{
    [field: SerializeField] public WishGranter Type { get; private set; }
    public WishGranter Key => Type;
    public bool IsIK { get; set; }

    public void OnReset()
    {
        
    }

    public void OnPool()
    {

    }
}