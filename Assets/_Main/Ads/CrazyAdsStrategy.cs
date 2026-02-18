using System;
using CrazyGames;
using UnityEngine;

public class CrazyAdsStrategy : AdsStrategy
{
    private bool isInit;
    public override void Init(PlayerInput playerInput)
    {
        if (!CrazySDK.IsAvailable)
        {
            Debug.Log($"crazy sdk is not available!");
            return;
        }
        CrazySDK.Init(() =>
        {
            isInit = true; 
            var systemInfo = CrazySDK.User.SystemInfo;
            // possible values: "desktop", "tablet", "mobile"
            Debug.Log(systemInfo.os.version);
            
            switch (systemInfo.os.version)
            {
                case "desktop":
                    break;
                case "tablet":
                    playerInput.ForceMobile();
                    break;
                case "mobile":
                    playerInput.ForceMobile();
                    break;
                default:
                    break;
            }
        });
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