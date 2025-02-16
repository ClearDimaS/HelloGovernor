using System;
using System.Collections;
using System.Collections.Generic;
using Zenject;

[Serializable]
public class PlayerData
{
    public DateTime registrationDate = DateTime.Today;
    public int skinIndex;
    public int money;
    public List<int> boughtSkins = new () { 0 };
    public List<string> completedTutorials = new();
}

public class PlayerDataRepository
{
    [Inject] private CacheManager cacheManager;


    public int Money { get; set; }
    public int SkinIndex { get; set; }
    public List<int> BoughtSkins { get; set; }

    public bool IsTutorialCompleted(string getKey)
    {
        throw new NotImplementedException();
    }

    public void SetTutorialCompleted(string getKey)
    {
        throw new NotImplementedException();
    }

    public void SaveAll()
    {

    }
}