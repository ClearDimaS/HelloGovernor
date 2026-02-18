
using System;
using CrazyGames;
using UnityEngine;

public abstract class AdsStrategy
{
    public abstract void Init(PlayerInput playerInput);
    public abstract void ShowInter();
}

// This sample demonstrates how to use the LevelPlay SDK to load and show ads in a Unity game.
public class AdvertisementsManager : MonoBehaviour
{
    private AdsStrategy adsStrategy;
    private void Awake()
    {
        var input = FindAnyObjectByType<PlayerInput>(FindObjectsInactive.Include);
#if UNITY_WEBGL
    #if YandexWeb
        adsStrategy = new YandexAdsStrategy();
    #elif CrazyWeb
        adsStrategy = new CrazyAdsStrategy();
    #endif
#else
        adsStrategy = new LevelPlayAdsStrategy();
#endif
        adsStrategy.Init(input);
    }

    public void ShowInterstitial()
    {
        adsStrategy.ShowInter();
    }
}

