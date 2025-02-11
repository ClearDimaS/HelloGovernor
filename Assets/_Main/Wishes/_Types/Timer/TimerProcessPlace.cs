using System;

public class TimerProcessPlace : ProcessPlace
{
    public override float ProcessTime => granter.FullProgressTime;

    protected TimerWishGranter granter;
    
    private void Start()
    {
        granter = GetComponentInParent<TimerWishGranter>();
    }

    public override bool CanAddProgress(CitizenController citizen)
    {
        return true;
    }
}