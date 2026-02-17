using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;
using Zenject;

public class LanguageSettings : MonoBehaviour
{
    [Inject] private DiContainer diContainer;
    
    [SerializeField] private Button openSelectButton;
    [SerializeField] private Button closeSelectButton;
    [SerializeField] private Image selectedIcon;
    [SerializeField] private LocalizedAssetTable assetTable;
    [SerializeField] private LocalizedAsset<Sprite> flag;
    
    [SerializeField] private GameObject content;
    
    [SerializeField] private LanguageElement languagePrefab;
    [SerializeField] private RectTransform languagesParent;

    private List<LanguageElement> languageElements;
    
    private void Awake()
    {
        languageElements = content.GetComponentsInChildren<LanguageElement>(true).ToList();
        foreach (var lang in languageElements)
        {
            Destroy(lang.gameObject);
        }
        languageElements.Clear();

        LocalizationManager.Instance.Locales.Subscribe(locales =>
        {
            if (locales == null)
            {
                return;
            }
            foreach (var locale in locales)
            {
                SpawnLocale(locale);
            }
        }).AddTo(this);
        LocalizationManager.Instance.CurrentLocale.Subscribe(currentLocale =>
        {
            if (currentLocale == null)
            {
                return;
            }
            
            SetLocaleImage(currentLocale);
            foreach (var newElement in languageElements)
            {
                newElement.SetSelected(currentLocale.Identifier.Code == newElement.Code);
            }
        });

        openSelectButton.onClick.AddListener(() =>
        {
            ShowLanguagesList();
        });
        closeSelectButton.onClick.AddListener(() =>
        {
            HideLanguagesList();
        });
    }

    private void OnEnable()
    {
        var currentLocale = LocalizationManager.Instance.CurrentLocale.Value;
        if (currentLocale == null)
        {
            return;
        }
        foreach (var newElement in languageElements)
        {
            newElement.SetSelected(currentLocale.Identifier.Code == newElement.Code);
        }
    }

    private void Start()
    {
        HideLanguagesList();
    }

    private void GetLocaleIcon(string identifierCode, Action<Sprite> onLoad)
    {
        LocalizationManager.Instance.GetAssetFor(assetTable, flag, identifierCode, onLoad);
    }
    
    private void SpawnLocale(Locale locale)
    {
        var newLanguageElementGO = diContainer.InstantiatePrefab(languagePrefab.gameObject, languagesParent);
        var newLanguageElement = newLanguageElementGO.GetComponent<LanguageElement>();
        
        GetLocaleIcon(locale.Identifier.Code, sprite =>
        {
            newLanguageElement.Init(
                locale.Identifier.Code,
                onSelect: () => SetLocale(locale.Identifier.Code), 
                icon: sprite);
        });
        newLanguageElement.SetSelected(locale == LocalizationManager.Instance.CurrentLocale.Value);

        languageElements.Add(newLanguageElement);
    }

    private void SetLocaleImage(Locale locale)
    {
        if (locale == null)
        {
            return;
        }

        GetLocaleIcon(locale.Identifier.Code, s => selectedIcon.sprite = s);
    }

    private void SetLocale(string code)
    {
        LocalizationManager.Instance.SetLocale(code);
    }

    private void HideLanguagesList()
    {
        content.gameObject.SetActiveOnce(false);
    }

    private void ShowLanguagesList()
    {
        content.gameObject.SetActiveOnce(true);
    }
}
