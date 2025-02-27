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

    #if UNITY_EDITOR
    public bool hasAssistant;
    public bool debug;
    #endif
    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
#if UNITY_EDITOR
       hasAssistant = wishAssistant != null;
#endif
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

        #if UNITY_EDITOR
        if (debug)
        {
            Debug.Log($"{visible} {wishAssistant == null} {!isOnPlayer} {CanAddItemToPlayer(player)}");
        }
        #endif
        if (visible && wishAssistant == null && !isOnPlayer && CanAddItemToPlayer(player))
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
            if (wishAssistant != null)
            {
                var playerDiff = wishAssistant.TransformRoot.position - ItemTakePlace.position;
                playerDiff.y = 0f;
                if (playerDiff.magnitude > takeRadius || GetOwner() == null)
                {
                    if (wishAssistant == player)
                    {
                        isOnPlayer = false;
                    }
                    RemoveAssistant(wishAssistant);
                }
            }
        }
    }

    private bool CanAddItemToPlayer(PlayerController playerController)
    {
        var source = GetItemSource();
        if (source == null)
        {
            Debug.LogError($"source null at: {transform.parent.parent.name}/{transform.parent.name}/{transform.name}");
            return false;
        }

        return playerController.HasItemOfType(source.GetPrefab());
    }

    protected abstract Sprite GetItemIcon();
    
    public abstract GenericCitizenItemSource GetItemSource();

    public Transform GetItemTakePlace()
    {
        var source = GetItemSource();
        if (source == null)
        {
            return null;
        }
        return source.TakePlace;
    }

    public Transform GetItemSpendPlace()
    {
        return ItemTakePlace;
    }

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