using System.Collections;
using Zenject;

public class PressConferenceGranter : UIWishGranter
{
    protected override bool CanAddProgress(CitizenController citizen)
    {
        return processPlaces.Length == processed.Count && base.CanAddProgress(citizen);
    }
}
