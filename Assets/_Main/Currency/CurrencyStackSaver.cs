using UnityEngine;

public class CurrencyStackSaver : SavableMonoBehaviour<CurrencyStackData>
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