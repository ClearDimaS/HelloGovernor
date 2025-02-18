using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class BankRewardElement : MonoBehaviour
{
    [SerializeField] private TMP_Text rewardText;
    
    [FormerlySerializedAs("addReward")] [SerializeField] private BankRewardChangeElement addBankReward;
    [FormerlySerializedAs("removeReward")] [SerializeField] private BankRewardChangeElement removeBankReward;
    
    [SerializeField] private RectTransform changeStartPlace;
    [SerializeField] private RectTransform changeEndPlace;

    private Queue<BankRewardChangeElement> addRewards = new();
    private Queue<BankRewardChangeElement> removeRewards = new();

    private void Start()
    {
        addRewards.Enqueue(addBankReward);
        removeRewards.Enqueue(removeBankReward);
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

    private void SafeAddNewPrefab(Queue<BankRewardChangeElement> rewardsQueue)
    {
        if (rewardsQueue.Count <= 1)
        {
            rewardsQueue.Enqueue(Instantiate(rewardsQueue.Peek(), rewardsQueue.Peek().transform.parent));
        }
    }
}