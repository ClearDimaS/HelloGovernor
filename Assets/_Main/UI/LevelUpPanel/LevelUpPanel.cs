using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LevelUpPanel : MonoBehaviour
{
    [Inject] private PlayerController playerController;
    [Inject] private UI_Manager uiManager;
    
    [SerializeField] private float showTime = 1f;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private UI_ElementAnimator newLevelPanel;
    [SerializeField] private Button claimRewarddButton;
    [SerializeField] private LevelUpRewardElement rewardElementMoney;
    [SerializeField] private LevelUpRewardElement rewardElementSpeed;
    [SerializeField] private LevelUpRewardElement rewardElementCapacity;

    private int newLevelIndex;
    private PlayerLevelData newLevelData;
    private int lastAddedMoneyCount;
    
    private void Awake()
    {
        newLevelPanel.AnimateBack(instant:true);
        
        claimRewarddButton.onClick.AddListener(() =>
        {
            claimRewarddButton.interactable = false;
            if (lastAddedMoneyCount > 0)
            {
                uiManager.GetPanel<UI_RewardsPanel>().SpawnUIMoney(lastAddedMoneyCount,
                    rewardElementMoney.IconRect,
                    () =>
                    {
                        playerController.XP_Controller.GrantLevelUpData(newLevelData, newLevelIndex, true);
                    });   
            }
            newLevelPanel.AnimateBack();
        });
    }

    public void Show(int newLevelIndex, PlayerLevelData newLevelData)
    {
        this.newLevelIndex = newLevelIndex;
        claimRewarddButton.interactable = true;
        this.newLevelData = newLevelData;
        newLevelPanel.Animate();
        levelText.text = (newLevelIndex + 1).ToString();
        claimRewarddButton.gameObject.SetActive(false);
        UniTask.Delay(TimeSpan.FromSeconds(showTime)).ContinueWith(() =>
        {
            claimRewarddButton.gameObject.SetActive(true);
        }).AddTo(gameObject);

        lastAddedMoneyCount = newLevelData.moneyReward.amount;
        
        rewardElementMoney.Refresh(newLevelData.moneyReward);
        rewardElementSpeed.Refresh(newLevelData.speedReward);
        rewardElementCapacity.Refresh(newLevelData.capacityReward);
    }
}