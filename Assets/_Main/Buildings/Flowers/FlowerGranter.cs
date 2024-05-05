using Zenject;

public class FlowerGranter : WishGranter
{
    [Inject] private GameConfig gameConfig;
    public override EWish Type => EWish.Flowers;
    public override float FullProgressTime => gameConfig.grantFlowerDuration;
    public override int Reward => gameConfig.flowerReward;
    
    protected override bool CanAddProgress(CitizenController citizen)
    {
        return citizen.WishesController.IsGranterAssistantServing(Type);
    }

    protected override void OnSuccessProcess(CitizenController citizen)
    {
        base.OnSuccessProcess(citizen);
        citizen.Interactor.ActivateObject(EInteractable.Flowers);
    }
}