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
    [SerializeField] private Gradient timerGradient;
    [SerializeField] private GameObject inPorgressGO;
    
    private float completionPause = 3f;
    private float robberyBustedTime = -10f;
    private float lastProgress = 0f;
    private bool isBusted;

    private void Start()
    {

    }

    private void Update()
    {
        if (Time.time - robberyBustedTime < completionPause)
        {
            return;
        }
 
        var isRobbery = thiefsManager.IsStealing && !thiefsManager.HasBusted();
        
        if (robberyRoot.activeSelf != isRobbery)
        {
            robberyRoot.SetActive(isRobbery);   
        }

        if (!inPorgressGO.activeSelf)
        {
            inPorgressGO.SetActive(true);
        }
        if (isRobbery)
        {
            var progressNormalized = thiefsManager.GetRobberyNormalizedProgress();
            robberyTimerFill.fillAmount = 1f - progressNormalized;
            robberyTimerFill.color = timerGradient.Evaluate(robberyTimerFill.fillAmount);
        }
    }
}