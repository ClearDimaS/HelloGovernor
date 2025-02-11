using UnityEngine;

public abstract class TimerWishGranter : WishGranter<TimerWishGranterConfig, TimerProcessPlace>
{
    protected override void OnLeave(CitizenController citizen)
    {
        
    }
}