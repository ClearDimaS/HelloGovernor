using System.Collections;
using UnityEngine;
using Zenject;

public class WishesController : MonoBehaviour
{
    [Inject] private WishesPool wishesPool;
    [Inject] private WishGrantersManager grantersManager;
    [Inject] private GameConfig gameConfig;

    [SerializeField] private CitizenController citizenController;

    private Wish currentWish;
    private Transform target;

    private void Update()
    {
        if (currentWish == null)
        {
            SetRandomWish();
        }

        if (currentWish != null)
        {
            if (!currentWish.HasOKGranter())
            {
                if (grantersManager.TryGetWorkingGranter(currentWish.Type, out WishGranter granter))
                {
                    currentWish.SetGranter(granter);   
                }
            }
        }
    }
    
    public bool HasAnyWishes()
    {
        return currentWish = null;
    }

    public void AddProgress(EWish type, float addProgress)
    {
        if (currentWish != null && currentWish.Type == type)
        {
            currentWish.AddProgress(addProgress);
        }
    }

    public bool IsProgressFull(EWish type)
    {
        return currentWish != null && currentWish.Type == type && currentWish.IsProgressFull;
    }
    
    private void SetRandomWish()
    {
        var wishChances = gameConfig.wishChances;
        wishChances.Shuffle();
        var wishType = wishChances[0].type;
        currentWish = wishesPool.GetElement();
        currentWish.Initialize(wishType, citizenController);
    }
}