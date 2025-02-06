public abstract class OperatableGranter : WishGranter
{
    protected OperatablePlace operatablePlace;

    protected override bool CanAddProgress(CitizenController citizen)
    {
        return operatablePlace.IsOperated;
    }
}