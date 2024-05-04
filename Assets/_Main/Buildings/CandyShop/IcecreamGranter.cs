using UnityEngine;
using Zenject;

public class IcecreamGranter : WishGranter
{
    [Inject] private GameConfig gameConfig;
    public override EWish Type => EWish.IceCream;
    public override float FullProgressTime => gameConfig.grantIcecreamDuration;
    public override int Reward => gameConfig.icecreamReward;
    
    protected override bool CanAddProgress(CitizenController citizen)
    {
        return true;
    }

    protected override void OnSuccessProcess(CitizenController citizen)
    {
        base.OnSuccessProcess(citizen);
        citizen.Interactor.ActivateObject(EInteractable.IceCream);
    }
}