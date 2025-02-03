using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CityProgressPanel : UI_Panel
{
    [Inject] private CameraManager cameraManager;
    [Inject] private UpgradablePricesManager pricesManager;
    [Inject] private GameConfig gameConfig;
    [Inject] private CompassManager compassManager;
    
    [SerializeField] private Image cityProgressImage;
    [SerializeField] private TMP_Text cityProgressText;

    [SerializeField] private RectTransform questRoot;
    [SerializeField] private Image progressImage;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button hintButton;
    [SerializeField] private float completionPause = 3f;

    private float completionTime = -4f;
    private CanvasGroup group;
    private UpgradableObject nextUpgradable;
    private int spentAmount = -1;
    private float lastProgress = 0f;

    private void Start()
    {
        lastProgress = pricesManager.GetProgress();
        ApplyProgress(lastProgress);
        hintButton.onClick.AddListener(ShowTargetHouse);
        group = GetComponent<CanvasGroup>();
        if (group == null)
        {
            group = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void ShowTargetHouse()
    {
        cameraManager.SetTarget(pricesManager.GetNextData().BuyPlace, 2f, distanceMult: gameConfig.hintCameraDistanceMult);
    }

    private void Update()
    {
        if (Time.time - completionTime < completionPause)
        {
            return;
        }
        var next = pricesManager.GetNextData();
        if (next == null || pricesManager.IsLast())
        {
            group.alpha = 0f;
            return;
        }
        if (nextUpgradable != next && !pricesManager.IsLast())
        {
            if (nextUpgradable == null)
            {
                compassManager.AddTarget(next.BuyPlace, ECompasTarget.NewPurchase);
                nextUpgradable = next;
                if (nextUpgradable.Level == 0)
                {
                    titleText.text = $"Buy {nextUpgradable.GetTitle()}";
                }
                else
                {
                    titleText.text = $"Upgrade {nextUpgradable.GetTitle()}";
                }
                spentAmount = -1;
            }
        }

        if (nextUpgradable != null && pricesManager.IsCurrentBought())
        {
            compassManager.RemoveTarget(nextUpgradable.BuyPlace, ECompasTarget.NewPurchase);   
            completionTime = Time.time;
            SoundManager.Instance.TaskComplete();
                
            questRoot.transform.DOScale(Vector3.one * 1.3f, completionPause / 5f).OnComplete(() =>
            {
                questRoot.transform.DOScale(Vector3.one, completionPause / 5f).OnComplete(() =>
                {
                    questRoot.transform.DOScale(Vector3.one * 1.3f, completionPause / 5f).OnComplete(() =>
                    {
                        questRoot.transform.DOScale(Vector3.one, completionPause / 5f).OnComplete(() =>
                        {
                            nextUpgradable = null;
                        });
                    });
                });
            });
        }

        if (nextUpgradable != null)
        {
            if (spentAmount < nextUpgradable.SpentAmount)
            {
                spentAmount = nextUpgradable.SpentAmount;
                progressImage.fillAmount = nextUpgradable.SpentAmount/(float)nextUpgradable.Price;
                progressText.text = $"{nextUpgradable.SpentAmount}/{nextUpgradable.Price}";
            }
        
            if (spentAmount > nextUpgradable.SpentAmount)
            {
                spentAmount = nextUpgradable.Price;
                progressImage.fillAmount = 1f;
                progressText.text = $"{nextUpgradable.Price}/{nextUpgradable.Price}";
            }   
        }
        
        var progress = pricesManager.GetProgress();
        if (progress != lastProgress)
        {
            var start = lastProgress;
            var end = progress;
            lastProgress = progress;
            var t = 0f;
            cityProgressText.transform.DOScale(Vector3.one * 1.2f, 0.3f);
            DOTween.To(() => t, x => t = x, 1f, 1f).OnUpdate(() =>
            {
                var value = Mathf.Lerp(start, end, t);
                ApplyProgress(value);
            }).OnComplete(() =>
            {
                var value = end;

                ApplyProgress(value);
                cityProgressText.transform.DOScale(Vector3.one * 1f, 0.3f);
            });
        }
    }

    private void ApplyProgress(float normalizedValue)
    {
        cityProgressImage.fillAmount = normalizedValue;
        cityProgressText.text = $"{Mathf.RoundToInt(normalizedValue*100)}%";
    }
}