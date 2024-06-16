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
    [SerializeField] private GameObject bustedGO;
    [SerializeField] private GameObject inPorgressGO;
    
    private float completionPause = 4f;
    private float robberyBustedTime = -10f;
    private float lastProgress = 0f;
    private bool isBusted;

    private void Start()
    {
        bustedGO.SetActive(false);
    }

    private void Update()
    {
        if (Time.time - robberyBustedTime < completionPause)
        {
            return;
        }
        if (isBusted)
        {
            if (!thiefsManager.IsActiveStealing)
            {
                return;
            }
            else
            {
                isBusted = false;
            }
        }
        
        var isRobbery = thiefsManager.IsStealing;
        
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
            var hasBusted = thiefsManager.HasBusted();
            if (hasBusted)
            {
                robberyTimerFill.gameObject.SetActive(false);
                isBusted = true;
                bustedGO.SetActive(true);
                inPorgressGO.SetActive(false);
                robberyRoot.transform.DOScale(Vector3.one * 1.3f, completionPause / 5f).OnComplete(() =>
                {
                    robberyRoot.transform.DOScale(Vector3.one, completionPause / 5f).OnComplete(() =>
                    {
                        robberyRoot.transform.DOScale(Vector3.one * 1.3f, completionPause / 5f).OnComplete(() =>
                        {
                            robberyRoot.transform.DOScale(Vector3.one, completionPause / 5f).OnComplete(() =>
                            {
                                robberyTimerFill.gameObject.SetActive(true);
                                isBusted = false;
                                bustedGO.SetActive(false);
                                robberyRoot.SetActive(false);   
                            });
                        });
                    });
                });
            }
            else
            {
                if (bustedGO.activeSelf)
                {
                    bustedGO.SetActive(false);
                }
            }
            var progressNormalized = thiefsManager.GetRobberyNormalizedProgress();
            robberyTimerFill.fillAmount = 1f - progressNormalized;
            robberyTimerFill.color = timerGradient.Evaluate(robberyTimerFill.fillAmount);
        }
    }
}