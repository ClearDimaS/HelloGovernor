using UnityEngine;
using Zenject;

public class IcecreamGranter : WishGranter
{
    [Inject] private GameConfig gameConfig;
    
    protected override bool CanAddProgress(CitizenController citizen)
    {
        return citizen.WishesController.IsGranterAssistantServing(this);
    }

    protected override void OnSuccessProcess(CitizenController citizen)
    {
        base.OnSuccessProcess(citizen);
    }
}