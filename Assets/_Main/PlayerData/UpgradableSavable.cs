using UnityEngine;

public class UpgradableSavable : SavableMonoBehaviour<UpgradableData>
{
    protected override string GetDataString(string key)
    {
        return PlayerPrefs.GetString(key, "");
    }

    protected override void SetDataString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }
}