using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoneyGiverBase : CulledBehaviour, ICooldownable
{
    [SerializeField] protected UpgradableBuilding building;
    [SerializeField] protected CurrencyStackBehaviour stack;
    [SerializeField] protected Animator enableWhenWorking;
    
    [SerializeField] protected int coolDown = 2;
    [SerializeField] protected int moneyPerCycle = 10;
    [SerializeField] protected int timer = 10;
    [SerializeField] protected int maxMoney = 1000;

    private bool isWorking;
    private float lastTimeSpawn;
    
    public float CoolDown => timer;

    public float CoolDownTimeLeft
    {
        get
        {
            var timeSpend = Time.time - lastTimeSpawn;
            var timeLeft = timer - timeSpend;
            return timeLeft;
        }
    }

    public bool IsCooldown => isWorking;

    protected override void OnOnEnable()
    {
        base.OnOnEnable();
        lastTimeSpawn = Time.time;
        enableWhenWorking.speed = 0f;
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (!building.IsBought)
        {
            return;
        }
        if (isWorking)
        {
            if (Time.time - lastTimeSpawn > timer)
            {
                isWorking = false;
                enableWhenWorking.speed = 0f;
                lastTimeSpawn = Time.time;
                AddMoney();
            }
        }
        else
        {
            if (Time.time - lastTimeSpawn > coolDown && stack.GetMoney() < maxMoney)
            {
                lastTimeSpawn = Time.time;
                isWorking = true;
                enableWhenWorking.speed = 1f;
            }
        }
    }

    private void AddMoney()
    {
        stack.AddCurrency(moneyPerCycle);
    }
}
