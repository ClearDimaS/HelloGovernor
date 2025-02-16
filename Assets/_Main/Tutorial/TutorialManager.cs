using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TutorialManager : Singleton<TutorialManager>
{
    [Inject] private GameConfig gameConfig;
    [Inject] private CameraManager cameraManager;
    [Inject] private PlayerDataRepository playerRepository;
    [Inject] private UpgradablePricesManager pricesManager;

    [SerializeField] private GameObject tutorialArrow;

    private List<TutorialStep> tutorialSteps;
    private int curStepIndex = 0;
    
    private void Start()
    {
        BuildTutorialSteps();
        for (int i = 0; i < tutorialSteps.Count; i++)
        {
            if (tutorialSteps[i].IsCompleted())
            {
                curStepIndex++;
            }
            else
            {
                break;
            }
        }

        if (curStepIndex < tutorialSteps.Count)
        {
            RefreshArrowTarget(tutorialSteps[curStepIndex]);   
        }
    }
    
    private void Update()
    {
        if (curStepIndex >= 0 && curStepIndex < tutorialSteps.Count)
        {
            var curStep = tutorialSteps[curStepIndex];
            curStep.UpdateProgress();
            if (curStep.IsCompleted())
            {
                curStep.SaveAsCompleted();
                curStepIndex++;
            }

            if (curStepIndex < tutorialSteps.Count)
            {
                var newStep = tutorialSteps[curStepIndex];
                ShowTargetPlace(callback: () =>
                {
                    if (newStep is BuildingTutorialStep)
                    {
                        pricesManager.AllowNext();
                    }
                });
                RefreshArrowTarget(newStep);   
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
        var show = arrowTarget != null;
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
            if (purchasable.upgradable is UpgradableBuilding building)
            {
                tutorialSteps.Add(new BuildingTutorialStep(building, purchasable.level-1, purchasable.thisTypeIndex, playerRepository));
            }

            var granter = purchasable.upgradable.GetComponent<WishGranter>();
            if (granter != null)
            {
                if (granter is OperatableGranter operatable)
                {
                    tutorialSteps.Add(new OperatableTutorialStep(operatable, playerRepository));
                }else
                if (granter is ItemsWishGranter itemsGranter)
                {
                    tutorialSteps.Add(new ItemsTakeTutorialStep(itemsGranter, playerRepository));
                    tutorialSteps.Add(new ItemsUseTutorialStep(itemsGranter, playerRepository));
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
        cameraManager.SetTarget(camTarget, 2f, distanceMult: gameConfig.hintCameraDistanceMult, startCallback:callback);  
    }
}
