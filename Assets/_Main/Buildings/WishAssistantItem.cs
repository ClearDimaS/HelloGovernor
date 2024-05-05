using UnityEngine;

public class WishAssistantItem : MonoBehaviour, IResetable, IKey<EWish>
{
    [field: SerializeField] public EWish Type { get; private set; }
    public EWish Key => Type;
    
    public void OnReset()
    {
        
    }

    public void OnPool()
    {

    }
}