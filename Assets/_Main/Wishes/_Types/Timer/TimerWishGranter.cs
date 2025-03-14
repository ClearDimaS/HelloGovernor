using UnityEngine;

public abstract class TimerWishGranter : WishGranter<TimerWishGranterConfig, TimerProcessPlace>
{
    protected override bool IsPlayerProcessing(CitizenController citizenController)
    {
        return false;
    }
    
    protected override void OnLeave(CitizenController citizen)
    {
        
    }
}