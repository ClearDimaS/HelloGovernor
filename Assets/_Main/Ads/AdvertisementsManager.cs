
using System;
using UnityEngine;

public abstract class AdsStrategy
{
    public abstract void Init();
    public abstract void ShowInter();
}

// This sample demonstrates how to use the LevelPlay SDK to load and show ads in a Unity game.
public class AdvertisementsManager : MonoBehaviour
{
    private AdsStrategy adsStrategy;
    private void Awake()
    {
#if UNITY_WEBGL
    #if YandexWeb
        adsStrategy = new YandexAdsStrategy();
    #elif CrazyWeb
        adsStrategy = new CrazyAdsStrategy();
    #endif
#else
        adsStrategy = new LevelPlayAdsStrategy();
#endif
        adsStrategy.Init();
    }

    public void ShowInterstitial()
    {
        adsStrategy.ShowInter();
    }
}

