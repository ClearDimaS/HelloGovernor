using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CityProgressPanel : UI_Panel
{
    [Inject] private TutorialManager tutorialManager;

    [SerializeField] private GameObject completeGO;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image progressImage;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button hintButton;
    [SerializeField] private float completionPause = 3f;
    [SerializeField] private GameObject content;
    
    private float completionTime = -4f;
    private CanvasGroup group;
    private TutorialStep lastStep;
    private float completedTimeout;

    private void Start()
    {
        hintButton.onClick.AddListener(ShowTarget);
        group = GetComponent<CanvasGroup>();
        if (group == null)
        {
            group = gameObject.AddComponent<CanvasGroup>();
        }

        completeGO.SetActiveOnce(false);
    }

    private void ShowTarget()
    {
        tutorialManager.ShowTargetPlace(null);
    }

    private void Update()
    {
        content.SetActiveOnce(tutorialManager.IsInit);
        if (!tutorialManager.IsInit)
        {
            return;
        }
        completeGO.SetActiveOnce(Time.time - completionTime < completionPause);
        if (Time.time - completionTime < completionPause)
        {
            return;
        }

        if (lastStep != null && lastStep.IsCompleted())
        {
            completionTime = Time.time;
            completeGO.SetActiveOnce(true);
        }
        var step = tutorialManager.GetCurrentStep();
        if (step != lastStep && step != null)
        {
            lastStep = step;
            titleText.text = step.GetTitle();
            iconImage.sprite = step.GetTutorialIcon();
        }
        if (lastStep != null)
        {
            progressImage.rectTransform.FillParent(lastStep.GetProgress());
            progressText.text = lastStep.GetProgressText();
        }
    }
}