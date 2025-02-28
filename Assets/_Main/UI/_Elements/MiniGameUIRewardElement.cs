using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class MiniGameUIRewardElement : MonoBehaviour
{
    [SerializeField] private TMP_Text rewardText;
    
    [FormerlySerializedAs("addBankReward")] [SerializeField] private RewardChangeElement addReward;
    [FormerlySerializedAs("removeBankReward")] [SerializeField] private RewardChangeElement removeReward;
    
    [SerializeField] private RectTransform changeStartPlace;
    [SerializeField] private RectTransform changeEndPlace;

    private Queue<RewardChangeElement> addRewards = new();
    private Queue<RewardChangeElement> removeRewards = new();

    private void Start()
    {
        addRewards.Enqueue(addReward);
        removeRewards.Enqueue(removeReward);
    }

    public void Init(int reward)
    {
        rewardText.text = reward.ToString();
    }

    public void RefreshWithChange(int totall, int change)
    {
        rewardText.transform.DOScale(Vector3.one * 1.15f, 0.15f).OnComplete(() =>
        {
            rewardText.text = totall.ToString();
            rewardText.transform.DOScale(Vector3.one, 0.15f);
        });
        rewardText.text = totall.ToString();
        if (change > 0)
        {
            SafeAddNewPrefab(addRewards);
            var rwrd = addRewards.Dequeue();
            rwrd.Init(changeStartPlace, changeEndPlace, change, () => addRewards.Enqueue(rwrd));
        }
        else if(change < 0)
        {
            SafeAddNewPrefab(removeRewards);
            var rwrd = removeRewards.Dequeue();
            rwrd.Init(changeStartPlace, changeEndPlace, change, () => removeRewards.Enqueue(rwrd));
        }
    }

    private void SafeAddNewPrefab(Queue<RewardChangeElement> rewardsQueue)
    {
        if (rewardsQueue.Count <= 1)
        {
            rewardsQueue.Enqueue(Instantiate(rewardsQueue.Peek(), rewardsQueue.Peek().transform.parent));
        }
    }
}