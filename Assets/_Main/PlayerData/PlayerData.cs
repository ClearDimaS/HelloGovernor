using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerDataRepository
{
    [Inject] private CacheManager cacheManager;


    public int Money
    {
        get => cacheManager.Money;
        set => cacheManager.Money = value;
    }
    public int SkinIndex 
    {
        get => cacheManager.SkinIndex;
        set => cacheManager.SkinIndex = value;
    }

    public bool IsSkinBought(int skin)
    {
        return cacheManager.BoughtSkins.Contains(skin);
    }
    
    public void SetSkinBought(int skin)
    {
        cacheManager.BoughtSkins.Add(skin);
        cacheManager.BoughtSkins = cacheManager.BoughtSkins;
    }

    public bool IsTutorialCompleted(string key)
    {
        return cacheManager.CompletedTutorials.Contains(key);
    }

    public void SetTutorialCompleted(string key)
    {
        cacheManager.CompletedTutorials.Add(key);
        cacheManager.CompletedTutorials = cacheManager.CompletedTutorials;
    }

    public void SaveAll()
    {

    }
}