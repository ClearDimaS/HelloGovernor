using System.Collections;
using UnityEngine;

public interface IDataHolder<T>
{
    public T GetData();
    public void Initialize(T data);
}

public abstract class SavableMonoBehaviour : MonoBehaviour, ISavable
{
    private void OnEnable()
    {
        SaveManager.Instance.AddSavable(this);
    }

    public abstract void SaveData();
    public abstract void LoadData();
}

public abstract class SavableMonoBehaviour<T> : SavableMonoBehaviour where T : new()
{
    private IDataHolder<T> dataHolder;
    private string keyCache;

    public string Key
    {
        get
        {
            if (string.IsNullOrEmpty(keyCache))
            {
                keyCache = CreateKey();
            }
            return keyCache;
        }
    }

    public override void LoadData()
    {
        if (dataHolder == null)
        {
            dataHolder = GetComponent<IDataHolder<T>>();
        }
        if (dataHolder == null)
        {
            Debug.LogWarning($"savable {Key} has no holder!");
            return;
        }

        var data = GetStorageData();
        dataHolder.Initialize(data);   
    }

    public override void SaveData()
    {
        var data = dataHolder.GetData();
        if (data == null)
        {
            data = new T();
        }

        SetStorageData(data);
    }

    protected abstract string GetDataString(string key);
    
    protected abstract void SetDataString(string key, string value);
    
    private string WithMaxLength(string value, int maxLength)
    {
        if (value == null)
        {
            return null;
        }
        if (maxLength < 0)
        {
            return "";
        }
        return value.Substring(0, Mathf.Min(value.Length, maxLength));
    }
    
    private string CreateKey()
    {
        var saveKey = "";
        var child = transform;
        var parent = transform.parent;
        saveKey += $"{WithMaxLength(child.name, 4)}{child.GetSiblingIndex()}";
        while (parent != null)
        {
            saveKey += $".{WithMaxLength(parent.name, 4)}{parent.GetSiblingIndex()}";
            child = parent;
            parent = parent.parent;
        }
        return saveKey;
    }
    
    private T GetStorageData()
    {
        var stringValue = GetDataString(Key);
        if (string.IsNullOrEmpty(stringValue))
        {
            return new T();
        }

        return JsonUtility.FromJson<T>(stringValue);
    }
    
    private void SetStorageData(T data)
    {
        var stringValue = JsonUtility.ToJson(data);
        SetDataString(Key, stringValue);
    }
}