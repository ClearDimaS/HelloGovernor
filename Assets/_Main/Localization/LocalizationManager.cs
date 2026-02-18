using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class LocalizationManager : Singleton<LocalizationManager>
{
    private PlayerPrefsStringRepository savedLocale;
    public ReactiveProperty<List<Locale>> Locales { get; private set; } = new();
    public ReactiveProperty<Locale> CurrentLocale { get; private set; } = new();

    private float lastChangeTime = 1f;
    
    protected override void OnCreated()
    {
        base.OnCreated();
        DontDestroyOnLoad(gameObject);
        StartCoroutine(Init());
        savedLocale = new PlayerPrefsStringRepository("locale_cache");
    }

    IEnumerator Init()
    {
        // Wait for the localization system to initialize
        yield return LocalizationSettings.InitializationOperation;

        // Get all available locales
        var locales = LocalizationSettings.AvailableLocales.Locales;
        CurrentLocale.Value = LocalizationSettings.SelectedLocale;
        
        foreach (var locale in locales)
        {
            // Locale.LocaleName gives you the language name (e.g., "English (en)")
            // Locale.Identifier.Code gives you the code (e.g., "en")
            Debug.Log($"Available Language: {locale.LocaleName} ({locale.Identifier.Code})");
        }
        Locales.Value = locales;
        
        SetLocale(savedLocale.Get());
    }

    public void SetLocale(string code)
    {
        Locale desiredLocale = LocalizationSettings.AvailableLocales.GetLocale(code);
        if (desiredLocale != null)
        {
            lastChangeTime = Time.time;
            LocalizationSettings.SelectedLocale = desiredLocale;
            CurrentLocale.Value = desiredLocale;
        }
        else
        {
            Debug.LogWarning("Locale not found for code: " + code);
        }

        savedLocale.Set(code);
    }

    public async UniTaskVoid GetAssetFor<T>(LocalizedAssetTable table, LocalizedAsset<T> tableEntry, string сode, 
        Action<T> onLoad) 
        where T: UnityEngine.Object
    {
        while (Locales == null || Locales.Value == null || Locales.Value.Count == 0)
        {
            await UniTask.Yield();
        }
        
        var locale = Locales.Value.First(x => x.Identifier.Code == сode);
        var op = LocalizationSettings.AssetDatabase.GetLocalizedAssetAsync<T>(
            table.TableReference, 
            tableEntry.TableEntryReference, 
            locale);
        
        while (!op.IsDone)
        {
            await UniTask.Yield();
        }

        onLoad(op.Result);
    }


    public bool IsLoadedQuests()
    {
        return Time.time - lastChangeTime > 1f;
    }
}
