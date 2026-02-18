using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class TutorialManager : Singleton<TutorialManager>
{
    [Inject] private TutorialsConfig tutorialsConfig;
    [Inject] private CompassManager compassManager;
    [Inject] private SkinChooser skinChooser;
    [Inject] private GameConfig gameConfig;
    [Inject] private CameraManager cameraManager;
    [Inject] private PlayerDataRepository playerRepository;
    [Inject] private UpgradablePricesManager pricesManager;

    [SerializeField] private float finishDelay = 2f;
    [SerializeField] private GameObject tutorialArrow;

    private List<TutorialStep> tutorialSteps;
    public GameObject ArrowGO => tutorialArrow;
    private int curStepIndex = 0;

    public List<TutorialStep> TutorialSteps => tutorialSteps;
    public int CurrentIndex => curStepIndex;
    public bool IsInit => isInit;

    private int skippedFrames;
    private bool isInit;
    private bool isMoneyCompassAdded;
    private bool isBusyWithAds;
    private AdvertisementsManager advertisementsManager;
    
    private void Start()
    {
        BuildTutorialSteps();
        advertisementsManager = FindAnyObjectByType<AdvertisementsManager>();
    }
    
    private void Update()
    {
        if (skippedFrames < 4)
        {
            skippedFrames++;
            return;
        }

        if (!isInit)
        {
            foreach (var step in tutorialSteps)
            {
                step.UpdateProgress();
                if (step.IsCompleted())
                {
                    curStepIndex++;
                }
                else
                {
                    break;
                }
            }

            isInit = true;
            if (curStepIndex < tutorialSteps.Count)
            {
                var newStep = tutorialSteps[curStepIndex];
                RefreshArrowTarget(tutorialSteps[curStepIndex]);   
                AnalyticsManager.Instance.StartLevel(curStepIndex);
                ShowTargetPlace(callback: () =>
                {
                    newStep.Start();
                    if (newStep is BuildingTutorialStep buildingTutorialStep)
                    {
                       
                    }
                });   
            }
        }

        if (!isMoneyCompassAdded && pricesManager.GetBoughtGranters().Count == 1)
        {
            isMoneyCompassAdded = true;
            var granter = pricesManager.GetBoughtGranters().ElementAt(0);
            compassManager.AddTarget(granter.GetStackPlace(), ECompasTarget.Money);
        }
        if (isMoneyCompassAdded && pricesManager.GetBoughtGranters().Count > 1)
        {
            isMoneyCompassAdded = false;
            var granter = pricesManager.GetBoughtGranters().ElementAt(0);
            compassManager.RemoveTarget(granter.GetStackPlace(), ECompasTarget.Money);
        }
        
        if (curStepIndex >= 0 && curStepIndex < tutorialSteps.Count)
        {
            var curStep = tutorialSteps[curStepIndex];
            curStep.UpdateProgress();
            if (curStep.IsCompleted() && cameraManager.IsOnPlayer && !cameraManager.IsBlocked)
            {
                if (isBusyWithAds)
                {
                    return;
                }
                
                AnalyticsManager.Instance.StartLevel(curStepIndex);
                AnalyticsManager.Instance.WinLevel(curStepIndex);
                curStep.SaveAsCompleted();
                curStepIndex++;
        
                if (curStepIndex < tutorialSteps.Count)
                {
                    var newStep = tutorialSteps[curStepIndex];
                    UniTask.Delay(TimeSpan.FromSeconds(finishDelay)).ContinueWith(() =>
                    {
                        ShowTargetPlace(callback: () =>
                        {
                            newStep.Start();
                        });   
                    });
                    RefreshArrowTarget(newStep); 
                    advertisementsManager.ShowInterstitial();
                }
                else
                {
                    RefreshArrowTarget(curStep); 
                }
            }

            if (curStepIndex < tutorialSteps.Count)
            {
                RefreshArrowTarget(GetCurrentStep());   
            }
        }
    }

    public TutorialStep GetCurrentStep()
    {
        if (curStepIndex >= 0 && curStepIndex < tutorialSteps.Count)
        {
            return tutorialSteps[curStepIndex];
        }
        return null;
    }
    
    private void RefreshArrowTarget(TutorialStep step)
    {
        var arrowTarget = step.GetArrowTarget();
        var show = arrowTarget != null && !step.IsCompleted();
        if (show)
        {
            if (!tutorialArrow.activeSelf)
            {
                tutorialArrow.SetActive(true);
            }
            tutorialArrow.transform.position = arrowTarget.position;   
        }
        else
        {
            if (tutorialArrow.activeSelf)
            {
                tutorialArrow.SetActive(false);
            }
        }
    }

    private bool wasFashionFound;
    
    private void BuildTutorialSteps()
    {
        tutorialSteps = new();
        foreach (var purchasable in pricesManager.GetPurchaseSequence())
        {
            if (purchasable.upgradable is UpgradableBuilding building && purchasable.level <= 1)
            {
                tutorialSteps.Add(new BuildingTutorialStep(
                    building, 
                    purchasable.level-1, 
                    purchasable.thisTypeIndex, 
                    playerRepository,
                    tutorialsConfig)
                );
                if (!wasFashionFound)
                {
                    var fashion = building.GetComponent<FashionBuilding>();
                    if (fashion != null)
                    {
                        wasFashionFound = true;
                        tutorialSteps.Add(new ChangeSkinTutorialStep(
                            skinChooser: skinChooser,
                            activationPlace: fashion.ActivationPlace,
                            repository:playerRepository,
                            tutorialsConfig));
                    }
                }
            }

            if (purchasable.upgradable is IncomeUpgrader incomeUpgrader)
            {
                tutorialSteps.Add(new IncomeUpgraderTutorialStep(
                    incomeUpgrader, 
                    purchasable.level-1,
                    purchasable.thisTypeIndex,
                    playerRepository,
                    tutorialsConfig)
                );
            }

            var granter = purchasable.upgradable.GetComponent<WishGranter>();
            if (granter != null)
            {
                if (granter is OperatableGranter operatable)
                {
                    tutorialSteps.Add(new OperatableTutorialStep(operatable, cameraManager, playerRepository,
                        tutorialsConfig));
                    var operatorUpgradable = granter.GetComponentInChildren<UpgradableOperator>();
                    if (operatorUpgradable != null)
                    {
                        tutorialSteps.Add(new AnyUpgradableTutorialStep(
                            operatorUpgradable,
                            0,
                            100,
                            playerRepository,
                            tutorialsConfig));
                    }
                }else
                if (granter is ItemsWishGranter itemsGranter)
                {
                    tutorialSteps.Add(new ItemsTakeTutorialStep(itemsGranter, playerRepository,
                        tutorialsConfig));
                    tutorialSteps.Add(new ItemsUseTutorialStep(itemsGranter, playerRepository,
                        tutorialsConfig));
                    
                    var assistantUpgradable = granter.GetComponentInChildren<UpgradableAssistant>();
                    if (assistantUpgradable != null)
                    {
                        tutorialSteps.Add(new AnyUpgradableTutorialStep(
                            assistantUpgradable,
                            0,
                            1000,
                            playerRepository,
                            tutorialsConfig));  
                    }
                }
                else
                if (granter is OrderWishGranter orderGranter)
                {
                    tutorialSteps.Add(new ItemsTakeTutorialStep(orderGranter, playerRepository,
                        tutorialsConfig));
                    tutorialSteps.Add(new ItemsUseTutorialStep(orderGranter, playerRepository,
                        tutorialsConfig));
                    
                    var assistantUpgradable = granter.GetComponentInChildren<UpgradableAssistant>();
                    if (assistantUpgradable != null)
                    {
                        tutorialSteps.Add(new AnyUpgradableTutorialStep(
                            assistantUpgradable,
                            0,
                            1000,
                            playerRepository,
                            tutorialsConfig));  
                    }
                }else
                if (granter is UIWishGranter uiGranter)
                {
                    tutorialSteps.Add(new UIGranterTutorialStep(uiGranter, playerRepository,
                        tutorialsConfig));
                }
            }
        }
    }

    public void ShowTargetPlace(Action callback)
    {
        var step = GetCurrentStep();
        if (step == null)
        {
            return;
        }
        var camTarget = step.GetCameraTarget();
        var sp = cameraManager.ActiveCamera.WorldToViewportPoint(camTarget.position);
        if ((sp.x < gameConfig.cameraUnlockXBorders.x ||
             sp.y < gameConfig.cameraUnlockYBorders.x ||
             sp.x > gameConfig.cameraUnlockXBorders.y || 
             sp.y > gameConfig.cameraUnlockYBorders.y))
        {
            cameraManager.SetTarget(camTarget, 2f, distanceMult: gameConfig.hintCameraDistanceMult, startCallback:callback);   
        }
        else
        {
            callback?.Invoke();
        }
    }
}
