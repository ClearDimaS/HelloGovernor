using UnityEngine;

public class BankAssistantSaver : SavableMonoBehaviour<BankAssistantData>
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