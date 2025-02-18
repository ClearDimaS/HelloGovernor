using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BankGame
{
    protected int reward = 0;

    private BankOption curVariant1;
    private BankOption curVariant2;
    public bool IsRunning => Time.time - startTime < duration;
    protected float startTime;
    protected float duration;
    
    public BankGame(float duration)
    {
        startTime = Time.time;
        this.duration = duration;
    }

    public float GetTimeLeft()
    {
        var timeSpent = Time.time - startTime;
        return duration - timeSpent;
    }
    
    public int GetReward()
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
        throw new NotImplementedException();
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
}