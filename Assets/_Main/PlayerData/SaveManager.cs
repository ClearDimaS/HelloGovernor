using System;
using System.Collections.Generic;
using UnityEngine;

public interface ISavable
{
    public void SaveData();
    public void LoadData();
}

public class SaveManager : Singleton<SaveManager>
{
    private HashSet<ISavable> savables = new (256);

    protected override void OnCreated()
    {
        base.OnCreated();
        Application.quitting += SaveAll;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
#if UNITY_EDITOR
        return;
#endif
        if (!hasFocus)
        {
            SaveAll();
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveAll();   
        }
    }

    private void OnApplicationQuit()
    {
        SaveAll();
    }

    private void SaveAll()
    {
        foreach (var savable in savables)
        {
            try
            {
                savable.SaveData();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"error saving: {savable}! {e.Message}");
                throw;
            }
        }
    }

    public void AddSavable(ISavable savable)
    {
        savables.Add(savable);
        savable.LoadData();
    }
}