using UnityEngine;

public abstract class OperatableGranter : WishGranter<OperatableWishGranterConfig, OperatableProcessPlace>
{
    [SerializeField] protected OperatablePlace operatablePlace;
    public int ServedCounter { get; set; }

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