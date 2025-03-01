using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Zenject;

public class CacheManager
{
    private PlayerPrefsListIntRepository boughtSkins = new ("boughtSkins");
    private PlayerPrefsListIntRepository grantedMoneys = new ("grantedMoneys");
    private PlayerPrefsListStringRepository completedTutorials = new ("completedTutorials");
    private PlayerPrefsIntRepository money = new("money");
    private PlayerPrefsIntRepository skinIndex = new("skin");
    
    private PlayerPrefsIntRepository vibrations = new("vibrations");
    private PlayerPrefsIntRepository sound = new("sound");
    private PlayerPrefsStringRepository registration = new("registration");
    private PlayerPrefsIntRepository levelIndex = new("levelIndex");
    private PlayerPrefsIntRepository xp = new("xp");

    [Inject]
    public CacheManager(GameConfig gameConfig)
    {
        if (!boughtSkins.Get().list.Contains(0))
        {
            boughtSkins.Set(new List<int>(){0});
        }

        if (PlayerPrefs.GetInt("IsInit", -1) == -1)
        {
            money.Set(gameConfig.startMoney);
            PlayerPrefs.SetInt("IsInit", 1);
            PlayerPrefs.Save();
        }
    }

    public int LevelIndex
    {
        get => levelIndex.Get();
        set => levelIndex.Set(value);
    }
    
    public int XP
    {
        get => xp.Get();
        set => xp.Set(value);
    }
    
    public bool IsVibrationsOn
    {
        get => vibrations.Get() == 0;
        set => vibrations.Set(value ? 0 : -1);
    }
    
    public bool IsSoundOn
    {
        get => sound.Get() == 0;
        set => sound.Set(value ? 0 : -1);
    }

    public DateTime RegistrationDate
    {
        get
        {
            var str = registration.Get();
            if (string.IsNullOrEmpty(str))
            {
                registration.Set(DateTime.Now.ToString(CultureInfo.InvariantCulture));
                return DateTime.Now;
            }
            if (DateTime.TryParse(str, out var date))
            {
                return date;
            }
            else
            {
                registration.Set(DateTime.Now.ToString(CultureInfo.InvariantCulture));
                return DateTime.Now;
            }
        }
        set => registration.Set(value.ToString(CultureInfo.InvariantCulture));
    }

    public int Money
    {
        get => money.Get();
        set => money.Set(value);
    }
    
    public int SkinIndex
    {
        get => skinIndex.Get();
        set => skinIndex.Set(value);
    }

    public List<int> BoughtSkins
    {
        get => boughtSkins.Get().list;
        set => boughtSkins.Set(value);
    }
    
    public List<int> GrantedMoneys
    {
        get => grantedMoneys.Get().list;
        set => grantedMoneys.Set(value);
    }

    public List<string> CompletedTutorials
    {
        get => completedTutorials.Get().list;
        set => completedTutorials.Set(value);
    }
}

public abstract class PlayerPrefsRepository<T>
{
    protected string key;

    public PlayerPrefsRepository(string key)
    {
        this.key = key;
    }

    public abstract void Set(T value);
    public abstract T Get();
}

public class PlayerPrefsSerializableRepository<T> : PlayerPrefsRepository<T>
{
    public PlayerPrefsSerializableRepository(string key) : base(key) { }

    public override void Set(T value)
    {
        PlayerPrefs.SetString(key, JsonUtility.ToJson(value));
        PlayerPrefs.Save();
    }

    public override T Get()
    {
        var str = PlayerPrefs.GetString(key, default(string));
        if (!string.IsNullOrEmpty(str))
        {
            return JsonUtility.FromJson<T>(str);   
        }
        return default;
    }
}

public class PlayerPrefsStringRepository : PlayerPrefsRepository<string>
{
    public PlayerPrefsStringRepository(string key) : base(key) { }

    public override void Set(string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
    }

    public override string Get()
    {
        return PlayerPrefs.GetString(key, default(string));
    }
}

[Serializable]
public class ListIntHolder
{
    public List<int> list = new ();
}

[Serializable]
public class ListBoolHolder
{
    public List<bool> list = new ();
}

[Serializable]
public class ListStringHolder
{
    public List<string> list = new ();
}

public class PlayerPrefsListStringRepository : PlayerPrefsRepository<ListStringHolder>
{
    private ListStringHolder cache;
    public PlayerPrefsListStringRepository(string key) : base(key) { }

    public void Set(List<string> valueList)
    {
        if (cache == null)
        {
            cache = Get();
        }
        cache.list = valueList;
        PlayerPrefs.SetString(key, JsonUtility.ToJson(cache));
        PlayerPrefs.Save();
    }
    
    public override void Set(ListStringHolder value)
    {
        cache = value;
        PlayerPrefs.SetString(key, JsonUtility.ToJson(value));
        PlayerPrefs.Save();
    }

    public override ListStringHolder Get()
    {
        if (cache == null)
        {
            var valueString =  PlayerPrefs.GetString(key, "");
            if (string.IsNullOrEmpty(valueString))
            {
                cache = new ListStringHolder();
            }
            else
            {
                cache = JsonUtility.FromJson<ListStringHolder>(valueString);
            }
        }

        return cache;
    }
}

public class PlayerPrefsListIntRepository : PlayerPrefsRepository<ListIntHolder>
{
    private ListIntHolder cache;
    public PlayerPrefsListIntRepository(string key) : base(key) { }

    public void Set(List<int> valueList)
    {
        if (cache == null)
        {
            cache = Get();
        }
        cache.list = valueList;
        PlayerPrefs.SetString(key, JsonUtility.ToJson(cache));
        PlayerPrefs.Save();
    }
    
    public override void Set(ListIntHolder value)
    {
        cache = value;
        PlayerPrefs.SetString(key, JsonUtility.ToJson(value));
        PlayerPrefs.Save();
    }

    public override ListIntHolder Get()
    {
        if (cache == null)
        {
            var valueString =  PlayerPrefs.GetString(key, "");
            if (string.IsNullOrEmpty(valueString))
            {
                cache = new ListIntHolder();
            }
            else
            {
                cache = JsonUtility.FromJson<ListIntHolder>(valueString);
            }
        }

        return cache;
    }
}


public class PlayerPrefsListBoolRepository : PlayerPrefsRepository<ListBoolHolder>
{
    private ListBoolHolder cache;
    public PlayerPrefsListBoolRepository(string key) : base(key) { }
    public string Key => key;

    public void Set(List<bool> valueList)
    {
        if (cache == null)
        {
            cache = Get();
        }
        cache.list = valueList;
        PlayerPrefs.SetString(key, JsonUtility.ToJson(cache));
        PlayerPrefs.Save();
    }
    
    public override void Set(ListBoolHolder value)
    {
        cache = value;
        PlayerPrefs.SetString(key, JsonUtility.ToJson(value));
        PlayerPrefs.Save();
    }

    public override ListBoolHolder Get()
    {
        if (cache == null)
        {
            var valueString =  PlayerPrefs.GetString(key, "");
            if (string.IsNullOrEmpty(valueString))
            {
                cache = new ListBoolHolder();
            }
            else
            {
                cache = JsonUtility.FromJson<ListBoolHolder>(valueString);
            }
        }

        return cache;
    }
}

public class PlayerPrefsIntRepository : PlayerPrefsRepository<int>
{
    public PlayerPrefsIntRepository(string key) : base(key) { }

    public override void Set(int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    public override int Get()
    {
        return PlayerPrefs.GetInt(key, default(int));
    }
}

public class PlayerPrefsFloatRepository : PlayerPrefsRepository<float>
{
    public PlayerPrefsFloatRepository(string key) : base(key) { }

    public override void Set(float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    public override float Get()
    {
        return PlayerPrefs.GetFloat(key, default(float));
    }
}

