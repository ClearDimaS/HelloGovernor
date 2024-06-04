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
}

public class PlayerDataRepository : Repository<PlayerData>
{
    [Inject] private GameConfig gameConfig;

    protected override PlayerData CreateClass()
    {
        var newObject = base.CreateClass();
        newObject.money = gameConfig.startMoney;
        return newObject;
    }
}