using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CurrencySource : MonoBehaviour
{
    [Inject] private CurrencySingleStackPool singleStacksPool;

    [SerializeField] private float force = 6f;
    [SerializeField] private int maxSpawned = 4;
    
    private List<CurrencySingleStackBehaviour> spawned = new ();

    public void GiveCurrency(int amount)
    {
        if (spawned.Count >= maxSpawned)
        {
            spawned[Random.Range(0, spawned.Count)].AddCurrency(amount);
        }
        else
        {
            var newStack = singleStacksPool.GetElement();
            spawned.Add(newStack);
            newStack.AddForce(GetRandomDir() * force);
        }
    }

    private Vector3 GetRandomDir()
    {
        return new Vector3(
            Random.Range(-1f, 1f), 
            1f, 
            Random.Range(-1f, 1f));
    }
}
