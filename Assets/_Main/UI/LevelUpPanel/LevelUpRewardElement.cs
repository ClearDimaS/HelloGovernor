using TMPro;
using UnityEngine;

public class LevelUpRewardElement : MonoBehaviour
{
    [SerializeField] private TMP_Text rewardText;

    public void Refresh(PlayerLevelRewardMoney reward)
    {
        rewardText.text = reward.GetStringValue();
        gameObject.SetActive(reward.amount > 0);
    }
    
    public void Refresh(PlayerLevelRewardSpeed reward)
    {
        rewardText.text = reward.GetStringValue();
        gameObject.SetActive(reward.addPercents > 0);
    }
    
    public void Refresh(PlayerLevelRewardCapacity reward)
    {
        rewardText.text = reward.GetStringValue();
        gameObject.SetActive(reward.add > 0);
    }
}