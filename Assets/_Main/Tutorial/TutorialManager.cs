using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class TutorialManager : Singleton<TutorialManager>
{
    [Inject] private GameConfig gameConfig;
    [Inject] private CameraManager cameraManager;
    [Inject] private PlayerDataRepository playerRepository;
    [Inject] private UpgradablePricesManager pricesManager;

    [SerializeField] private float finishDelay = 2f;
    [SerializeField] private GameObject tutorialArrow;

    private List<TutorialStep> tutorialSteps;
    private int curStepIndex = 0;

    private bool wasAssistantAdded;
    private bool wasOperatorAdded;
    private int skippedFrames;
    private bool isInit;
    
    private void Start()
    {
        BuildTutorialSteps();
    }
    
    private void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.U))
        {
            var stepToSkip = tutorialSteps[curStepIndex];
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
        #endif
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

                ShowTargetPlace(callback: () =>
                {
                    newStep.Start();
                    if (newStep is BuildingTutorialStep buildingTutorialStep)
                    {
                       
                    }
                });   
            }
        }
        if (curStepIndex >= 0 && curStepIndex < tutorialSteps.Count)
        {
            var curStep = tutorialSteps[curStepIndex];
            curStep.UpdateProgress();
            if (curStep.IsCompleted())
            {
                curStep.SaveAsCompleted();
                curStepIndex++;
        
                if (curStepIndex < tutorialSteps.Count)
                {
                    var newStep = tutorialSteps[curStepIndex];
                    UniTask.Delay(TimeSpan.FromSeconds(finishDelay)).ContinueWith(() =>
                    {
                        ShowTargetPlace(callback: () =>
                        {
  
                        });   
                    });
                    RefreshArrowTarget(newStep); 
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
                    playerRepository)
                );
            }

            if (purchasable.upgradable is IncomeUpgrader incomeUpgrader)
            {
                tutorialSteps.Add(new IncomeUpgraderTutorialStep(
                    incomeUpgrader, 
                    purchasable.level-1,
                    purchasable.thisTypeIndex,
                    playerRepository)
                );
            }

            var granter = purchasable.upgradable.GetComponent<WishGranter>();
            if (granter != null)
            {
                if (granter is OperatableGranter operatable)
                {
                    tutorialSteps.Add(new OperatableTutorialStep(operatable, playerRepository, cameraManager));
                    if (!wasOperatorAdded)
                    {
                        var operatorUpgradable = granter.GetComponentInChildren<UpgradableOperator>();
                        if (operatorUpgradable != null)
                        {
                            tutorialSteps.Add(new AnyUpgradableTutorialStep(
                                operatorUpgradable,
                                0,
                                100,
                                playerRepository));
                            wasOperatorAdded = true;   
                        }
                    }
                }else
                if (granter is ItemsWishGranter itemsGranter)
                {
                    tutorialSteps.Add(new ItemsTakeTutorialStep(itemsGranter, playerRepository));
                    tutorialSteps.Add(new ItemsUseTutorialStep(itemsGranter, playerRepository));
                    
                    if (!wasAssistantAdded)
                    {
                        var assistantUpgradable = granter.GetComponentInChildren<UpgradableAssistant>();
                        if (assistantUpgradable != null)
                        {
                            tutorialSteps.Add(new AnyUpgradableTutorialStep(
                                assistantUpgradable,
                                0,
                                1000,
                                playerRepository));
                            wasAssistantAdded = true;   
                        }
                    }
                }else
                if (granter is UIWishGranter uiGranter)
                {
                    tutorialSteps.Add(new UIGranterTutorialStep(uiGranter, playerRepository));
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
