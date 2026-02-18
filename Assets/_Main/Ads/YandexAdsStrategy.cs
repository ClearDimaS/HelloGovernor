using YG;

public class YandexAdsStrategy : AdsStrategy
{
    public override void Init(PlayerInput input)
    {
        if (YG2.Device.Mobile  == YG2.infoYG.Simulation.device || YG2.Device.Tablet == YG2.infoYG.Simulation.device)
        {
            input.ForceMobile();
        }
    }

    public override void ShowInter()
    {
        YG.YG2.InterstitialAdvShow();
    }
}