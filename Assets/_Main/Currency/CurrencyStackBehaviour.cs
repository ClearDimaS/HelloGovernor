using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public interface ICurrencyHolder
{
    public void AddCurrency(int amount);

    public void MoveCurrencyToMe(CurrencyBehaviour currency);
}

[Serializable]
public class CurrencyStackData
{
    public int modelsCount;
    public int moneyAmount;
}

public class CurrencyStackBehaviour : SimplePlayerPhysicsBehaviour, ICurrencyHolder, IDataHolder<CurrencyStackData>
{
    [Inject] private CameraManager cameraManager;
    [Inject] private PlayerController player;
    [Inject] private GameConfig gameConfig;
    [Inject] private CurrencyPool currencyPool;
    [Inject] private CurrencySingleStackPool currencySinglePool;

    [SerializeField] private bool allowOverflow = false;
    [SerializeField] private GameObject takeZone;
    [SerializeField] private CurrencyPlacer gridPlacer;

    private WishGranter granter;
    private static CurrencyStackBehaviour instance;
    private CurrencyStackData saveData;

    public int GetMoney()
    {
        if (saveData == null)
        {
            return 0;
        }
        return saveData.moneyAmount;
    }
    private void Start()
    {
        var fwd = cameraManager.ActiveCamera.transform.forward;
        fwd.y = 0f;
        fwd = fwd.normalized;
        transform.rotation = Quaternion.LookRotation(-fwd, Vector3.up);
        granter = GetComponentInParent<WishGranter>();
    }
    
    public void Initialize(CurrencyStackData data)
    {
        if (instance == null)
        {
            instance = this;
        }
        saveData = data;

        var amount = saveData.moneyAmount;
        var given = 0;
        var max = Mathf.Min(saveData.modelsCount, gridPlacer.MaxPlaces);
        for (int i = 0; i < max; i++)
        {
            var toGive = amount / saveData.modelsCount;
            if (i == saveData.modelsCount - 1)
            {
                toGive = amount - given;
            }

            given += toGive;
            CurrencyBehaviour currency = currencyPool.GetElement();
            currency.Init(toGive);
            gridPlacer.Add(currency, true);
        }
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        var hasAny = saveData.moneyAmount > 0;
        if (takeZone.activeSelf != hasAny)
        {
            takeZone.gameObject.SetActive(hasAny);
        }

        if (visible && IsInside(player) && gridPlacer.Count > 1)
        {
            Remove(player);
        }
        saveData.moneyAmount = gridPlacer.GetMoneyAmount();
    }

    public CurrencyStackData GetData()
    {
        saveData.modelsCount = gridPlacer.Count;
        saveData.moneyAmount = gridPlacer.GetMoneyAmount();
      
        return saveData;
    }

    protected override void OnEnter(PlayerController component)
    {
        base.OnEnter(component);
        Remove(component);
    }

    public void AddCurrency(int reward)
    {
        var models = Mathf.Clamp(reward / gameConfig.moneyInOneModel, 1, gameConfig.moneyRewardMaxModels);
        if (gridPlacer.CanAddOneMore())
        {
            for (int i = 0; i < models; i++)
            {
                var currency = currencyPool.GetElement();
                if (i == 0)
                {
                    currency.Init(reward);
                }
                gridPlacer.Add(currency);
                if (!gridPlacer.CanAddOneMore())
                {
                    break;
                }
            }
        }
        else
        {
            gridPlacer.GetLast().AddAmount(reward);
        }
    }

    public void MoveCurrencyToMe(int amount, Vector3 worldStart)
    {
        if (IsFull() && !allowOverflow)
        {
            return;
        }

        var moneyInOneModel = GetMoneyInOneModel();

        var maxAdd = GetMaxMoney() - GetMoney();
        amount = Mathf.Min(maxAdd, amount);
        
        var oldAmount = GetMoney();
        var newAmount = amount + oldAmount;
        
        var oldModelCount = oldAmount / moneyInOneModel;
        var newModelCount = newAmount / moneyInOneModel;
        var modelCount = newModelCount - oldModelCount;

        if (modelCount <= 0)
        {
            modelCount = 1;
        }

        var givenAmount = 0;
        for (int i = 0; i < modelCount; i++)
        {
            var toGive = amount / modelCount;
            if (i == modelCount - 1)
            {
                toGive = amount - givenAmount;
            }
            givenAmount += toGive;
            if (gridPlacer.Count < gridPlacer.MaxPlaces)
            {
                CurrencyBehaviour currency = currencyPool.GetElement();
                currency.Init(toGive);
                currency.transform.position = worldStart;
                gridPlacer.Add(currency);
            }
            else if(allowOverflow)
            {
                var currency = gridPlacer.GetLast();
                currency.AddAmount(toGive);
            }
        }
    }

    private int GetMoneyInOneModel()
    {
        var mult = 1f;
        if (granter != null)
        {
            mult *= granter.IncomeMultiplier;
        }

        return Mathf.RoundToInt(gameConfig.moneyInOneModel * mult);
    }

    public void MoveCurrencyToMe(CurrencyBehaviour currency)
    {
        gridPlacer.Add(currency);
    }

    public void Remove(ICurrencyHolder target)
    {
        var count = gridPlacer.Count;
        var pause = gameConfig.moneyFromStackPause;
        var time = gameConfig.moneyFromStackMaxTime;
        pause = Mathf.Min(pause, time / count);
        for (int i = 0; i < count; i++)
        {
            var delay = i * pause;
            var currency = gridPlacer.Remove();
            if (delay > 0f)
            {
                UniTask.Delay(TimeSpan.FromSeconds(delay)).ContinueWith(() =>
                {
                    target.MoveCurrencyToMe(currency);
                });
            }
            else
            {
                target.MoveCurrencyToMe(currency);   
            }
        }
    }
    
    public void RemoveOne(ICurrencyHolder target)
    {
        var count = gridPlacer.Count;
        if (count > 0)
        {
            var currency = gridPlacer.Remove();
            target.MoveCurrencyToMe(currency);
        }
    }

    public bool IsFull()
    {
        return !gridPlacer.CanAddOneMore();
    }

    public static void SpawnSingleCurrency(int count, Vector3 pos)
    {
        var reward = count;
        var currency = instance.currencySinglePool.GetElement();
        currency.transform.position = pos + Vector3.up;
        currency.Initialize(reward);
        currency.AddForce(Vector3.up * 2.3f + new Vector3(1f, 0, 1f).AxisToRandomDir() * 1);
    }

    public int GetMaxMoney()
    {
        return gridPlacer.MaxPlaces * GetMoneyInOneModel();
    }
}