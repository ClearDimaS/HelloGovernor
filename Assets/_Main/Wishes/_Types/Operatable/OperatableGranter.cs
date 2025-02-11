using UnityEngine;

public abstract class OperatableGranter : WishGranter<OperatableWishGranterConfig, OperatableProcessPlace>
{
    [SerializeField] protected OperatablePlace operatablePlace;

    public Sprite GetIconOperator()
    {
        return null;
    }

    public bool IsOperated()
    {
        return operatablePlace.IsOperated;
    }
    
    protected override void OnLeave(CitizenController citizen)
    {

    }
}