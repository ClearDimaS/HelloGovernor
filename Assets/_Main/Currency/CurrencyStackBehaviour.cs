using System;
using System.Collections;
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

public class CurrencyStackBehaviour : MonoBehaviour, ICurrencyHolder, IDataHolder<CurrencyStackData>
{
    [Inject] private GameConfig gameConfig;
    [Inject] private CurrencyPool currencyPool;
    
    [SerializeField] private CurrencyPlacer gridPlacer;

    private CurrencyStackData saveData;
    public void Initialize(CurrencyStackData data)
    {
        saveData = data;

        var amount = saveData.moneyAmount;
        var given = 0;
        for (int i = 0; i < saveData.modelsCount; i++)
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
    
    public CurrencyStackData GetData()
    {
        saveData.modelsCount = gridPlacer.Count;
        saveData.moneyAmount = gridPlacer.GetMoneyAmount();
        return saveData;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || other.attachedRigidbody == null)
        {
            return;
        }
        if (!other.attachedRigidbody.TryGetComponent(out PlayerController player))
        {
            return;
        }

        Remove(player);
    }

    public void AddCurrency(int reward)
    {
        if (gridPlacer.CanAddOneMore())
        {
            var currency = currencyPool.GetElement();
            currency.Init(reward);
            gridPlacer.Add(currency);
        }
    }

    public void MoveCurrencyToMe(int amount, Vector3 worldStart)
    {
        var  modelCount = amount / gameConfig.moneyInOneModel;
        if (modelCount == 0)
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
            else
            {
                var currency = gridPlacer.GetLast();
                currency.AddAmount(toGive);
            }
        }
    }
    
    public void MoveCurrencyToMe(CurrencyBehaviour currency)
    {
        gridPlacer.Add(currency);
    }

    public void Remove(ICurrencyHolder target)
    {
        var count = gridPlacer.Count;
        for (int i = 0; i < count; i++)
        {
            var currency = gridPlacer.Remove();
            target.MoveCurrencyToMe(currency);
        }
    }
}