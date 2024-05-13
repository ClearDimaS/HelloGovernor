using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CityProgressPanel : UI_Panel
{
    [Inject] private UpgradablePricesManager pricesManager;
    
    [SerializeField] private Image cityProgressImage;
    [SerializeField] private TMP_Text cityProgressText;

    private float lastProgress = 0f;

    private void Start()
    {
        lastProgress = pricesManager.GetProgress();
        ApplyProgress(lastProgress);
    }

    private void Update()
    {
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