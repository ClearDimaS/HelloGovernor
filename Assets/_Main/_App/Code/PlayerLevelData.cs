using System;
using UnityEngine;

[Serializable]
public class PlayerLevelData
{
    public int maxXP;
    public PlayerLevelRewardMoney moneyReward;
    public PlayerLevelRewardSpeed speedReward;
    public PlayerLevelRewardCapacity capacityReward;
}


[Serializable]
public class PlayerLevelRewardMoney
{
    public int amount;
    public string GetStringValue()
    {
        return $"+{amount}";
    }

    public void Grant(PlayerDataRepository repository)
    {
        repository.Money += amount;
    }
}
[Serializable]
public class PlayerLevelRewardSpeed
{
    public int addPercents;
   
    public string GetStringValue()
    {
        return $"+{addPercents}%";
    }
    
    public void Grant(PlayerController controller, bool isNew)
    {
        controller.AddSpeedPercents(addPercents, isNew);
    }
}
[Serializable]
public class PlayerLevelRewardCapacity
{
    public int add;

    public string GetStringValue()
    {
        return $"+{add}";
    }
    
    public void Grant(PlayerController controller, bool isNew)
    {
        controller.AddCapacity(add, isNew);
    }
}