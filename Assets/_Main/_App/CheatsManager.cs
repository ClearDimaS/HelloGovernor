using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CheatsManager : MonoBehaviour
{
    [Inject] private CameraManager cameraManager;
    [Inject] private PlayerDataRepository playerRepository;
    [Inject] private UI_Manager uiManager;

    [SerializeField] private Button close;
    [SerializeField] private GameObject view;
    [SerializeField] private Button[] activateCheatButtons;

    private int activateCheatIndex = -1;
    private bool isCinematicMode;
    private float startFOV;
    
    private void Start()
    {
        /*gameObject.SetActive(false);*/
        #if !DEVELOPMENT 
        Destroy(gameObject);
        return;
        #endif
        close.onClick.AddListener(() => view.SetActive(false));

        for (var i = 0; i < activateCheatButtons.Length; i++)
        {
            var btn = activateCheatButtons[i];
            var index = i;
            btn.onClick.AddListener(() =>
            {
                activateCheatIndex = index;
            });
        }
    }

    private void Update()
    {
        /*return;*/
        if (Input.touchCount > 3 || Input.GetKeyDown(KeyCode.Space))
        {
            view.SetActive(true);
        }
        var tutorialManager = TutorialManager.Instance;
        if (Input.GetKeyDown(KeyCode.U) || activateCheatIndex == 0)
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
            else if (stepToSkip is ChangeSkinTutorialStep skinStep)
            {
                skinStep.ForcePurchase();
            }

            activateCheatIndex = -1;
        }

        if (Input.GetKeyDown(KeyCode.M) || activateCheatIndex == 1)
        {
            activateCheatIndex = -1;
            playerRepository.Money += 5000;
        }
        
        if (Input.GetKeyDown(KeyCode.C) || activateCheatIndex == 2)
        {
            activateCheatIndex = -1;
            if (!isCinematicMode)
            {
                isCinematicMode = true;
                startFOV = cameraManager.ActiveCamera.fieldOfView;
                cameraManager.ActiveCamera.fieldOfView = 30f;
                uiManager.GetComponentInParent<CanvasGroup>().alpha = 0f;
                cameraManager.AllowChangeTarget(false);
                tutorialManager.ArrowGO.transform.localScale = Vector3.zero;
            }
            else
            {
                isCinematicMode = false;
                cameraManager.ActiveCamera.fieldOfView = startFOV;
                uiManager.GetComponentInParent<CanvasGroup>().alpha = 1f;
                cameraManager.AllowChangeTarget(true);
                tutorialManager.ArrowGO.transform.localScale = Vector3.one;
            }
        }
    }
}