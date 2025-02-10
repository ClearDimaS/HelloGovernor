using UnityEngine;

public abstract class OperatableGranter : WishGranter
{
    [SerializeField] protected OperatablePlace operatablePlace;

    protected override bool CanAddProgress(CitizenController citizen)
    {
        return operatablePlace.IsOperated;
    }
}