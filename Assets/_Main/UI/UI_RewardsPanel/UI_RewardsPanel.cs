using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UI_RewardsPanel : UI_Panel
{
    [Inject] private CameraManager cameraManager;
    [Inject] private UI_Manager uiManager;

    [SerializeField] private int xpMaxCounts = 10;
    [SerializeField] private int moneyMaxCounts = 15;
    [SerializeField] private float maxMoveMult = 1.2f;
    [SerializeField] private RewardUIElementsPool moneyRewardsPool;
    [SerializeField] private RewardUIElementsPool xpRewardsPool;
    
    private Vector2 moneyVPTarget;
    private Vector2 xpVPTarget;

    private void Start()
    {
        var moneyPlace = uiManager.GetPanel<WalletPanel>().MoneyPlace;
        var spMoney = RectTransformUtility.WorldToScreenPoint(moneyPlace.GetRenderCamera(), moneyPlace.position);
        moneyVPTarget = spMoney.SP_To_VP();
        
        var xpPlace = uiManager.GetPanel<LevelPanel>().XP_Place;
        var spXP = RectTransformUtility.WorldToScreenPoint(xpPlace.GetRenderCamera(), xpPlace.position);
        xpVPTarget = spXP.SP_To_VP();
    }

    public void SpawnUIMoney(int count, RectTransform from, Action onComplete)
    {
        var spPos = RectTransformUtility.WorldToScreenPoint(from.GetRenderCamera(), from.transform.position);
        var vpPos = spPos.SP_To_VP();
        
        for (int i = 0; i < moneyMaxCounts; i++)
        {
            var element = moneyRewardsPool.GetElement();

            var onEndMove = i == 0 ? onComplete : null;
            var mult = Mathf.Lerp(1, maxMoveMult, i/(float)moneyMaxCounts);
            element.Init(mult, vpPos, moneyVPTarget, onEndMove, moneyRewardsPool);
        }
    }
    
    public void SpawnXP(int count, Vector3 worldPos, Action onComplete)
    {
        var countClamped = Mathf.Clamp(count, 0, xpMaxCounts);
        var vpPos = cameraManager.ActiveCamera.WorldToViewportPoint(worldPos);
        
        for (int i = 0; i < countClamped; i++)
        {
            var element = xpRewardsPool.GetElement();
            var onEndMove = i == 0 ? onComplete : null;
            var mult = Mathf.Lerp(1, maxMoveMult, i/(float)countClamped);
            element.Init(mult, vpPos, xpVPTarget, onEndMove, xpRewardsPool);
        }
    }
}