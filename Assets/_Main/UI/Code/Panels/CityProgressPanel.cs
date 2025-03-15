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
    }

    private void ShowTarget()
    {
        tutorialManager.ShowTargetPlace(null);
    }

    private void Update()
    {
        completeGO.SetActiveOnce(Time.time - completionTime < completionPause);
        if (Time.time - completionTime < completionPause)
        {
            return;
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

        if (step != null && step.IsCompleted())
        {
            completionTime = Time.time;
            completedTimeout += Time.deltaTime;
        }
        else
        {
            completedTimeout = 0f;
        }
    }
}