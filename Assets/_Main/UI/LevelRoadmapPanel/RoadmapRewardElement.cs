using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoadmapRewardElement : MonoBehaviour
{
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private GameObject receivedGO;
    
    public void Refresh(PlayerLevelRewardMoney reward, bool received)
    {
        receivedGO.SetActive(received);
        rewardText.text = reward.GetStringValue();
        gameObject.SetActive(reward.amount > 0);
    }
    
    public void Refresh(PlayerLevelRewardSpeed reward, bool received)
    {
        receivedGO.SetActive(received);
        rewardText.text = reward.GetStringValue();
        gameObject.SetActive(reward.addPercents > 0);
    }
    
    public void Refresh(PlayerLevelRewardCapacity reward, bool received)
    {
        receivedGO.SetActive(received);
        rewardText.text = reward.GetStringValue();
        gameObject.SetActive(reward.add > 0);
    }
}