using System.Collections;
using UnityEngine;
using Zenject;

public interface ICurrencyHolder
{
    public void AddCurrency(int amount);

    public void MoveCurrencyToMe(CurrencyBehaviour currency);
}
public class CurrencyStackBehaviour : MonoBehaviour, ICurrencyHolder
{
    [Inject] private CurrencyPool currencyPool;
    
    [SerializeField] private CurrencyPlacer gridPlacer;
    
    public void AddCurrency(int reward)
    {
        if (gridPlacer.CanAddOneMore())
        {
            var currency = currencyPool.GetElement();
            currency.Init(reward);
            gridPlacer.Add(currency);
        }
    }

    public void MoveCurrencyToMe(CurrencyBehaviour currency)
    {
        throw new System.NotImplementedException();
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