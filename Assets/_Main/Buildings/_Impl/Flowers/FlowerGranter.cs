using Zenject;

public class FlowerGranter : WishGranter
{
    public override EWish Type => EWish.Flowers;
    
    protected override bool CanAddProgress(CitizenController citizen)
    {
        return citizen.WishesController.IsGranterAssistantServing(Type);
    }

    protected override void OnSuccessProcess(CitizenController citizen)
    {
        base.OnSuccessProcess(citizen);
    }
}