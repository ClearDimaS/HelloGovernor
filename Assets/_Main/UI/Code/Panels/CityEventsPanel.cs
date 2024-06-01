using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CityEventsPanel : UI_Panel
{
    [Inject] private ThiefsManager thiefsManager;
    
    [SerializeField] private GameObject robberyRoot;
    [SerializeField] private Image robberyTimerFill;

    private float lastProgress = 0f;

    private void Start()
    {

    }

    private void Update()
    {
        var isRobbery = thiefsManager.IsStealing;
        
        if (robberyRoot.activeSelf != isRobbery)
        {
            robberyRoot.SetActive(isRobbery);   
        }

        if (isRobbery)
        {
            var progressNormalized = thiefsManager.GetRobberyNormalizedProgress();
            robberyTimerFill.fillAmount = 1f - progressNormalized;
        }
    }
}