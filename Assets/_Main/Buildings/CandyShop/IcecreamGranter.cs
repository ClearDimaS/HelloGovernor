using UnityEngine;
using Zenject;

public class IcecreamGranter : WishGranter
{
    [Inject] private GameConfig gameConfig;
    public override EWish Type => EWish.IceCream;
    
    protected override bool CanAddProgress(CitizenController citizen)
    {
        return citizen.WishesController.IsGranterAssistantServing(Type);
    }

    protected override void OnSuccessProcess(CitizenController citizen)
    {
        base.OnSuccessProcess(citizen);
    }
}