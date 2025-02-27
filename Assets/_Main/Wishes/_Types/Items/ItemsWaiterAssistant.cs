using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public interface IItemsUserWishGranter
{
    ItemsUserProcessPlace GetProcessedWithoutAssistant();
    Transform GetIdlePlace();
    Transform GetPlaceToLookAt();
    float ProcessPlaceUserTime { get; }
}


public class ItemsWaiterAssistant : MonoBehaviour, IWishAssistant, IItemTaker
{
    [SerializeField] private Interactor interactor;
    [SerializeField] private Walker walker;
    [SerializeField] private float rotSpeed = 360f;

    private GenericCitizenItem item;
    private IItemsUserWishGranter wishGranter;
    private ItemsUserProcessPlace target;
    
    public Transform Root => transform;
    public Transform TransformRoot => transform;
    
    private void Awake()
    {
        wishGranter = GetComponentInParent<IItemsUserWishGranter>();
    }

    private void Update()
    {
        if (target != null)
        {
            if (target.Assistant != null && target.Assistant != this)
            {
                target = null;
            }
        }

        if (target == null)
        {
            RefreshTarget();
            walker.MoveToTarget(wishGranter.GetIdlePlace().position, null);
            if (!walker.IsMoving)
            {
                var diff = wishGranter.GetPlaceToLookAt().position - transform.position;
                diff.y = 0f;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(diff.normalized, Vector3.up),
                    rotSpeed * Time.deltaTime);
            }
        }
        if (target != null)
        {
            if (item == null)
            {
                target.RemoveAssistant(this);
                var source = target.GetItemSource();
                walker.MoveToTarget(target.GetItemTakePlace().position, () => source.AddTaker(this));
            }
            else
            {
                walker.MoveToTarget(target.GetItemSpendPlace().position, AllowAddProgressToWisher, 0.8f);   
            }

            if (target != null && target.GetOwner() == null)
            {
                target = null;
            }
        }
    }

    public bool CanAddItems()
    {
        return item == null;
    }

    public void AddItem(GenericCitizenItem getElement)
    {
        item = getElement;
        interactor.AddItem(item);
    }
    
    private void AllowAddProgressToWisher()
    {
        if (target != null && (target.Assistant == null))
        {
            target.SetWishAssistant(this);   
        }
        else
        {
            target = null;
        }
    }

    private void RefreshTarget()
    {
        target = wishGranter.GetProcessedWithoutAssistant();
    }
    
    public GenericCitizenItem RemoveItem()
    {
        var removed = item;
        interactor.RemoveItem();
        item = null;
        return removed;
    }

    public bool HasAnyItems()
    {
        return item != null;
    }
}
