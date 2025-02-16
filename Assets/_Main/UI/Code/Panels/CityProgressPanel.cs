using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CityProgressPanel : UI_Panel
{
    [Inject] private TutorialManager tutorialManager;

    [SerializeField] private GameObject progressRoot;
    [SerializeField] private CityProgressStep[] progressSteps;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image progressImage;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button hintButton;
    [SerializeField] private float completionPause = 3f;

    private float completionTime = -4f;
    private CanvasGroup group;
    private TutorialStep lastStep;

    private void Start()
    {
        hintButton.onClick.AddListener(ShowTarget);
        group = GetComponent<CanvasGroup>();
        if (group == null)
        {
            group = gameObject.AddComponent<CanvasGroup>();
        }

        RefreshProgressSequence();
    }

    private void RefreshProgressSequence()
    {
        for (var i = 0; i < progressSteps.Length; i++)
        {
            var step = progressSteps[i];
            var progressMarker = tutorialManager.GetProgressMarker(i);
            if (progressMarker == null)
            {
                if (i == 0)
                {
                    progressRoot.SetActive(false);
                }
                step.gameObject.SetActive(false);
            }
            else
            {
                if (!progressRoot.activeSelf)
                {
                    progressRoot.SetActive(true);   
                }
                step.gameObject.SetActive(true);
                step.Init(progressMarker.sprite, progressMarker.orderNumber);
            }
        }
    }

    private void ShowTarget()
    {
        tutorialManager.ShowTargetPlace(null);
    }

    private void Update()
    {
        if (Time.time - completionTime < completionPause)
        {
            return;
        }

        var step = tutorialManager.GetCurrentStep();
        if (step != lastStep)
        {
            RefreshProgressSequence();
            lastStep = step;
            titleText.text = step.GetTitle();
            iconImage.sprite = step.GetTutorialIcon();
        }
        if (lastStep != null)
        {
            progressImage.fillAmount = lastStep.GetProgress();
            progressText.text = lastStep.GetProgressText();
        }
    }
}