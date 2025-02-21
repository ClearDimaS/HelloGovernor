using System;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

public class ItemsProcessPlace : ProcessPlace, IItemsUserProcessPlace
{
    [Inject] private PlayerController player;
    
    [field: SerializeField] public Transform ItemTakePlace { get; private set; }
    [SerializeField] private TimerBase takeTimer;
    [SerializeField] private float takeRadius = 1.5f;
    
    private ItemsWishGranter itemsWishGranter;
    private IWishAssistant wishAssistant;
    private static bool isOnPlayer;

    public override float ProcessTime => itemsWishGranter.FullProgressTime;
    public IWishAssistant Assistant => wishAssistant;

    private void Start()
    {
        itemsWishGranter = GetComponentInParent<ItemsWishGranter>(true);
        takeTimer.SetIcon(itemsWishGranter.GetItemIcon());
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
        if (visible && wishAssistant == null && !isOnPlayer && player.HasItemOfType(itemsWishGranter.GetPrefabs()[0]))
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