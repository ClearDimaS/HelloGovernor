using UnityEngine;
using Zenject;

public class CheatsManager : MonoBehaviour
{
    [Inject] private CameraManager cameraManager;
    [Inject] private PlayerDataRepository playerRepository;
    [Inject] private UI_Manager uiManager;

    private bool isCinematicMode;
    private float startFOV;
    
    private void Start()
    {
#if !UNITY_EDITOR
Destroy(gameObject);
#endif
    }

    private void Update()
    {
#if UNITY_EDITOR
        var tutorialManager = TutorialManager.Instance;
        if (Input.GetKeyDown(KeyCode.U))
        {
            var stepToSkip = tutorialManager.TutorialSteps[tutorialManager.CurrentIndex];
            stepToSkip.SaveAsCompleted();
            if (stepToSkip is BuildingTutorialStep buildingTutorialStep)
            {
                buildingTutorialStep.ForcePurchase();
            }
            else if (stepToSkip is AnyUpgradableTutorialStep upgradable)
            {
                upgradable.ForcePurchase();
            }else if (stepToSkip is IncomeUpgraderTutorialStep incomeUpgrader)
            {
                incomeUpgrader.ForcePurchase();
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            playerRepository.Money += 5000;
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!isCinematicMode)
            {
                isCinematicMode = true;
                startFOV = cameraManager.ActiveCamera.fieldOfView;
                cameraManager.ActiveCamera.fieldOfView = 30f;
                uiManager.GetComponentInParent<CanvasGroup>().alpha = 0f;
            }
            else
            {
                isCinematicMode = false;
                cameraManager.ActiveCamera.fieldOfView = startFOV;
                uiManager.GetComponentInParent<CanvasGroup>().alpha = 1f;
            }
        }
#endif
    }
}