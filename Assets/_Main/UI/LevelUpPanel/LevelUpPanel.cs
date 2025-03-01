using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpPanel : MonoBehaviour
{
    [SerializeField] private float showTime = 1f;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private UI_ElementAnimator newLevelPanel;
    [SerializeField] private Button claimRewarddButton;
    [SerializeField] private LevelUpRewardElement rewardElementMoney;
    [SerializeField] private LevelUpRewardElement rewardElementSpeed;
    [SerializeField] private LevelUpRewardElement rewardElementCapacity;
    
    private void Awake()
    {
        newLevelPanel.AnimateBack(instant:true);
        
        claimRewarddButton.onClick.AddListener(() =>
        {
            newLevelPanel.AnimateBack();
        });
    }

    public void Show(int newLevelIndex, PlayerLevelData newLevelData)
    {
        newLevelPanel.Animate();
        levelText.text = (newLevelIndex + 1).ToString();
        claimRewarddButton.gameObject.SetActive(false);
        UniTask.Delay(TimeSpan.FromSeconds(showTime)).ContinueWith(() =>
        {
            claimRewarddButton.gameObject.SetActive(true);
        }).AddTo(gameObject);
        
        rewardElementMoney.Refresh(newLevelData.moneyReward);
        rewardElementSpeed.Refresh(newLevelData.speedReward);
        rewardElementCapacity.Refresh(newLevelData.capacityReward);
    }
}