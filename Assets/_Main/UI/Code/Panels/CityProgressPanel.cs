using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CityProgressPanel : UI_Panel
{
    [Inject] private CameraManager cameraManager;
    [Inject] private GameConfig gameConfig;
    [Inject] private TutorialManager tutorialManager;

    [SerializeField] private RectTransform questRoot;
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
            lastStep = step;
            titleText.text = step.GetTitle();
        }
        if (lastStep != null)
        {
            progressImage.fillAmount = lastStep.GetProgress();
            progressText.text = lastStep.GetProgressText();
        }
    }
}