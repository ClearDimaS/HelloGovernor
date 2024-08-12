using System;
using UnityEngine;


[Serializable]
public class UpgradableData
{
    public int spentMoney;
    public int level;
    public int optionIndex;
}

public class UpgradableSavable : SavableMonoBehaviour<UpgradableData>
{
    protected override string GetDataString(string key)
    {
        return PlayerPrefs.GetString(key, "");
    }

    protected override void SetDataString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }
}