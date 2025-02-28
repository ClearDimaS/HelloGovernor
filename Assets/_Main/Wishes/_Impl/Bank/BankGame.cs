using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BankGame : Minigame
{
    protected int reward = 0;

    private BankOption curVariant1;
    private BankOption curVariant2;
    public bool IsRunning => timer.GameTimeLeft > 0f;

    protected WishGranterTimer timer;
    protected WishesCollectionConfig gameConfig;
    private Action<int> onComplete;
    
    public BankGame(WishGranterTimer timer, WishesCollectionConfig gameConfig, Action<int> onComplete)
    {
        this.timer = timer;
        this.gameConfig = gameConfig;
        this.onComplete = onComplete;
    }

    public float GetTimeLeft()
    {
        return timer.GameTimeLeft;
    }
    
    public override int GetReward()
    {
        return reward;
    }

    public void GetNextOptions(out BankOption bankOption1, out BankOption bankOption2)
    {
        var isFirstNegative = Random.Range(0, 1f) < 0.5f;
        curVariant1 = CreateRandomVariant(isFirstNegative);
        curVariant2 = CreateRandomVariant(!isFirstNegative);
        bankOption1 = curVariant1;
        bankOption2 = curVariant2;
    }

    private BankOption CreateRandomVariant(bool allowNegative)
    {
        var isNegative = false;
        if (allowNegative)
        {
            isNegative = Random.Range(0, 1f) < gameConfig.bankNegativeProbability;
        }

        var minMax = isNegative ? gameConfig.bankRewardMinMaxNegative : gameConfig.bankRewardMinMaxPositive;
        var newReward = Random.Range(Mathf.Abs(minMax.x), Mathf.Abs(minMax.y));
        if (isNegative)
        {
            newReward = -newReward;
        }

        var typeInt = Random.Range(0, 4);
        var type = (EBankOption)typeInt;
        var option = BankOption.CreateFromType(type, newReward);
        return option;
    }

    public void SetSelected(BankOption data)
    {
        if (curVariant1 == data || curVariant2 == data)
        {
            curVariant2 = null;
            curVariant1 = null;
            reward += data.GetReward();   
        }
    }

    public override void Complete()
    {
        var reward = GetReward();
        reward = Mathf.Max(0, reward);
        onComplete?.Invoke(reward);
    }
}