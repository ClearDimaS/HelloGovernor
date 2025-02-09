using System.Collections;
using Zenject;

public class PressConferenceGranter : OperatableGranter
{

    protected override bool CanAddProgress(CitizenController citizen)
    {
        return WishPlaces.Length == processed.Count && base.CanAddProgress(citizen);
    }
}
