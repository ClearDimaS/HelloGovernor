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
        Debug.Log($"next: {next}  / {nextUpgradable}");
        if (nextUpgradable != next)
        {
            if (nextUpgradable == null)
            {
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

        if (nextUpgradable != null && pricesManager.IsCurrentBought(nextUpgradable))
        {
            pricesManager.AllowNext();
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
                            spentAmount = -1;
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
    }
}