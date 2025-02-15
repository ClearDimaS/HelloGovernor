using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TutorialManager : Singleton<TutorialManager>
{
    [Inject] private UpgradablePricesManager pricesManager;

    [SerializeField] private GameObject tutorialArrow;

    private void Update()
    {
        var data = pricesManager.GetNextData();
        var show = data != null;
        if (show)
        {
            if (!tutorialArrow.activeSelf)
            {
                tutorialArrow.SetActive(true);
            }
            tutorialArrow.transform.position = data.BuyPlace.position;   
        }
        else
        {
            if (tutorialArrow.activeSelf)
            {
                tutorialArrow.SetActive(false);
            }
        }
    }
}
