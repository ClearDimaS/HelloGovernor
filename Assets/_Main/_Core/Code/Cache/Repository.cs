using System;
using UnityEngine;
using Zenject;

public class Repository<T>
{
    [Inject] protected LocalCacheManager cacheManager;
    
    protected T stageData;

    public T GetData()
    {
        if (stageData == null)
        {
            return LoadExistingOrCreateTemplateLocalData();
        }

        return stageData;
    }

    public void SetData(T data)
    {
        stageData = data;
        var save = cacheManager.Save<T>(data, true);
    }
    
    protected T LoadExistingOrCreateTemplateLocalData()
    {
        if (cacheManager.FileExists<T>())
        {
            stageData = cacheManager.Load<T>();
            if (stageData == null)
            {
                cacheManager.Clear<T>();
                LoadExistingOrCreateTemplateLocalData();
            }
        }
        else if (stageData != null)
        {

        }
        else
        {
            stageData = CreateClass();
            if (stageData == null)
                Debug.LogError($"creating class null! {typeof(T)}");
            var saved = cacheManager.Save<T>(stageData, true);
            if (!saved)
                Debug.LogError($"saving locally error! {typeof(T)}");
        }
        return stageData;
    }
    
    protected virtual T CreateClass()
    {
        return (T)Activator.CreateInstance(typeof(T));
    }
}