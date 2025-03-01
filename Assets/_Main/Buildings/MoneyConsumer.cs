using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public interface IMoneySpender
{
    public Transform Root { get; }
    public void Spend(int diff);
    public int MaxToSpend();
}

[Serializable]
public class Price
{
    public int price = 10;
    public int spent;
    
    public bool IsBought => spent >= price;
    
    public Price(int price, int spent)
    {
        this.price = price;
        this.spent = spent;
    }

    public Price GetCopy()
    {
        return new Price(price, spent);
    }

    public static string ToMoneyString(int money)
    {
        if (money > 100_000_000)
        {
            return (money / 1_000_000_000f).ToString("0") + "B";
        }
        if (money > 10_000_000)
        {
            return (money / 1_000_000_000f).ToString("0.0") + "B";
        }
        if (money > 1_000_000)
        {
            return (money / 1_000_000f).ToString("0.00") + "M";
        }
        else if (money > 100_000)
        {
            return (money / 1000f).ToString("0") + "K";
        }
        else if (money > 10_000)
        {
            return (money / 1000f).ToString("0.0") + "K";
        }
        else if (money > 1000)
        {
            return (money / 1000f).ToString("0.00") + "K";
        }
        else
        {
            return money.ToString();
        }
    }
}

public class MoneyConsumer : SimplePlayerPhysicsBehaviour
{
    [Inject] private SoundManager soundManager;
    [Inject] private PlayerInput playerInput;
    [Inject] private CurrencyPool currencyPool;
    [Inject] private GameConfig gameConfig;
    [Inject] private CameraManager cameraManager;

    [SerializeField] private ScaleAnimator scaleAnimator;
    [SerializeField] private Transform flyTarget;
    
    private Price price;
    private IMoneySpender spender;

    private bool isReached;
    private int currentSpendAmount;
    private float currentSpendingTime;
    private float lastSpawnCashTime;

    public event Action reachGoalEvent;
    public int Price => price.price;

    private void Start()
    {
        var fwd = cameraManager.ActiveCamera.transform.forward;
        fwd.y = 0f;
        fwd = fwd.normalized;
        transform.rotation = Quaternion.LookRotation(-fwd, Vector3.up);
    }

    public void SetPrice(Price price)
    {
        isReached = false;
        this.price = price;
    }

    protected override void OnEnter(PlayerController component)
    {
        base.OnEnter(component);
        SetSpender(component);
    }

    protected override void OnLeave(PlayerController component)
    {
        base.OnLeave(component);
        spender = null;
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (visible)
        {
            RefreshSpendMoney();
        }
    }

    private void RefreshSpendMoney()
    {
        if (spender == null || price == null)
        {
            return;
        }
        if (price.IsBought)
        {
            if (!isReached)
            {
                isReached = true;
                spender = null;
                soundManager.PlayPurchase();
                reachGoalEvent?.Invoke();
            }
            return;
        }

        if (playerInput.HasRecentPresser())
        {
            return;
        }
        
        currentSpendingTime += Time.deltaTime;
        
        int maxAllowedCurrentAmount = Mathf.RoundToInt(currentSpendingTime / gameConfig.buyTime * price.price);

        var diff = maxAllowedCurrentAmount - currentSpendAmount;
        diff = Mathf.Min(diff, spender.MaxToSpend());
        diff = Mathf.Min(diff, price.price - GetSpentAmount());
        if (diff > 0)
        {
            soundManager.PlaySpendMoney();
            spender.Spend(diff);
            if ((Time.time - lastSpawnCashTime) > gameConfig.moneySpendPause)
            {
                lastSpawnCashTime = Time.time;
                SpawnFlyingCurrencyModel();
            }
        }
        price.spent += diff;
        currentSpendAmount = maxAllowedCurrentAmount;
    }

    private void SpawnFlyingCurrencyModel()
    {
        var currency = currencyPool.GetElement();
        currency.transform.position = spender.Root.position;
        var intermediateDuration = gameConfig.moneyFlyTime1;
        var t = 0f;
        var endRot = Quaternion.Euler(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f));
        var endPos = (flyTarget.position + spender.Root.position) / 2f + Vector3.up * gameConfig.moneySpendFlyHeight;
        var startPos = currency.transform.position;
        var startRot = currency.transform.rotation;
        DOTween.To(() => t, x => t = x, 1f, intermediateDuration).OnUpdate(() =>
        {
            currency.transform.position = Vector3.Lerp(startPos, endPos, t);
            currency.transform.rotation = Quaternion.Lerp(startRot, endRot, t);
        }).OnComplete(() =>
        {
            currency.transform.DOMove(flyTarget.position, gameConfig.moneyFlyTime2).SetEase(gameConfig.moneyFlyEase2);
            currency.transform.DOScale(Vector3.zero, gameConfig.moneyFlyTime2).SetEase(gameConfig.moneyFlyEase2).OnComplete(() => currencyPool.Pool(currency));
        }).SetEase(gameConfig.moneyFlyEase1);
    }

    private void SetSpender(IMoneySpender moneySpender)
    {
        spender = moneySpender;
        currentSpendAmount = 0;
        currentSpendingTime = 0f;
    }

    public int GetSpentAmount()
    {
        if (price == null)
        {
            Debug.LogWarning($"price is null at: {transform.name}  {transform.GetInstanceID()}  {transform.parent.name}");
            return 0;
        }
        return price.spent;
    }

    public void Hide(bool instant)
    {
        scaleAnimator.Hide(instant);
    }

    public void Show(bool instant)
    {
        scaleAnimator.Show(instant);
    }

    public int GetLeftAmount()
    {
        if (price == null)
        {
            return 0;
        }
        return price.price - price.spent;
    }
}