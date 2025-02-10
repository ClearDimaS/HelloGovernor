using UnityEngine;

public abstract class OperatableGranter : WishGranter<OperatableWishGranterConfig, OperatableProcessPlace>
{
    [SerializeField] protected OperatablePlace operatablePlace;

    protected override bool CanAddProgress(CitizenController citizen)
    {
        return operatablePlace.IsOperated;
    }

    public Sprite GetIconOperator()
    {
        return null;
    }
}