using UnityEngine;


public abstract class OperatableGranter : WishGranter<OperatableWishGranterConfig, OperatableProcessPlace>
{
    [SerializeField] protected OperatablePlace operatablePlace;
    public int ServedCounter { get; set; }
    
    protected override void OnAwake()
    {
        base.OnAwake();
        operatablePlace.SetShowExtraCondition(() =>
        {
            var owner = processPlaces[0].GetOwner();
            return owner != null && !owner.Walker.IsMoving;
        });
    }

    public Sprite GetIconOperator()
    {
        return config.operatorIcon;
    }
    
    public Sprite GetIconTimer()
    {
        return config.timerIcon;
    }

    public bool IsOperated()
    {
        return operatablePlace.IsOperated;
    }
    
    protected override void OnLeave(CitizenController citizen)
    {
        ServedCounter++;
    }

    public OperatablePlace GetOperatedPlace()
    {
        return operatablePlace;
    }
}