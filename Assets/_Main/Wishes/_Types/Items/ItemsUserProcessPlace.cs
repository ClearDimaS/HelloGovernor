using System;
using UnityEngine;
using Zenject;

public abstract class ItemsUserProcessPlace : ProcessPlace
{
    [Inject] private PlayerController player;
    
    [field: SerializeField] public Transform ItemTakePlace { get; private set; }
    [SerializeField] private TimerBase takeTimer;
    [SerializeField] private float takeRadius = 1.5f;
    
    private IItemsUserWishGranter itemsWishGranter;
    private IWishAssistant wishAssistant;
    private static bool isOnPlayer;

    public override float ProcessTime => itemsWishGranter.ProcessPlaceUserTime;
    public IWishAssistant Assistant => wishAssistant;

    private void Start()
    {
        itemsWishGranter = GetComponentInParent<IItemsUserWishGranter>(true);
        takeTimer.SetIcon(GetItemIcon());
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        takeTimer.SetProgress(progress);
        var showGFX = GetOwner() != null;
        if (showGFX != ItemTakePlace.gameObject.activeSelf)
        {
            ItemTakePlace.gameObject.SetActive(showGFX);
        }
        var showTimer = progress >= 0f && progress < 1f && GetOwner() != null;
        if (takeTimer.gameObject.activeSelf != showTimer)
        {
            takeTimer.gameObject.SetActive(showTimer);
        }
        if (visible && wishAssistant == null && !isOnPlayer && player.HasItemOfType(GetItemPrefab()))
        {
            var playerDiff = player.transform.position - ItemTakePlace.position;
            playerDiff.y = 0f;
            if (playerDiff.magnitude < takeRadius)
            {
                isOnPlayer = true;
                SetWishAssistant(player);
            }
        }
        else
        {
            if (isOnPlayer && wishAssistant == player)
            {
                var playerDiff = player.transform.position - ItemTakePlace.position;
                playerDiff.y = 0f;
                if (playerDiff.magnitude > takeRadius || GetOwner() == null)
                {
                    RemoveAssistant(player);
                    isOnPlayer = false;
                }
            }
        }
    }

    protected abstract GenericCitizenItem GetItemPrefab();

    protected abstract Sprite GetItemIcon();
    
    public abstract GenericCitizenItemSource GetItemSource();

    public abstract Transform GetItemTakePlace();

    public abstract Transform GetItemSpendPlace();

    public void SetWishAssistant(IWishAssistant waiterAssistant)
    {
        wishAssistant = waiterAssistant;
    }
    
    public bool HasAssistant()
    {
        return wishAssistant != null;
    }

    public GenericCitizenItem RemoveItem()
    {
        return wishAssistant.RemoveItem();
    }
    
    public void RemoveAssistant(IWishAssistant assistant)
    {
        if (assistant == wishAssistant)
        {
            wishAssistant = null;   
        }
    }
    
    public override bool CanAddProgress(CitizenController citizen)
    {
        return wishAssistant != null && wishAssistant.HasAnyItems();
    }
}