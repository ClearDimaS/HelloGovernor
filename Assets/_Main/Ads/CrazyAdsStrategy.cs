using CrazyGames;
using UnityEngine;

public class CrazyAdsStrategy : AdsStrategy
{
    private bool isInit;
    public override void Init()
    {
        if (!CrazySDK.IsAvailable)
        {
            Debug.Log($"crazy sdk is not available!");
            return;
        }
        CrazySDK.Init(() => { isInit = true; });
    }

    public override void ShowInter()
    {
        if (!isInit)
        {
            return;
        }
        CrazySDK.Ad.RequestAd(CrazyAdType.Midgame, () => // or CrazyAdType.Rewarded
        {
            Time.timeScale = 0f;
        }, (error) =>
        {
            Time.timeScale = 1f;
        }, () =>
        {
            Time.timeScale = 1f;
        });
    }
}