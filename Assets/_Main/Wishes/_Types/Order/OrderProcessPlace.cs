using System.Collections.Generic;
using UnityEngine;

public class OrderProcessPlace : ProcessPlace
{
    [SerializeField] private Transform itemTakePlace;
    [SerializeField] private OrderWishDisplayer[] displayers;
    [SerializeField] private Vector2Int itemsCountMinMax;
    private OrderWishGranter wishGranter;
    protected List<GenericCitizenItem> items = new ();
    protected OrderData orderData;

    public override float ProcessTime => wishGranter.FullProgressTime;

    protected override void OnAwake()
    {
        wishGranter = GetComponentInParent<OrderWishGranter>();
        base.OnAwake();
        orderData = new OrderData();
        CreateNewOrder();
    }

    public override bool CanAddProgress(CitizenController citizen)
    {
        return items.Count == orderData.items.Count;
    }

    public override void LeavePlace(CitizenController citizen)
    {
        base.LeavePlace(citizen);
        foreach (var item in items)
        {
            citizen.AddItem(item);
        }

        CreateNewOrder();
    }

    private void CreateNewOrder()
    {
        var itemsCount = Random.Range(itemsCountMinMax.x, itemsCountMinMax.y+1);
        orderData.Reinit(); 
        for (int i = 0; i < itemsCount; i++)
        {
            orderData.AddItem(wishGranter.GetRandomItem());
        }

        for (var i = 0; i < displayers.Length; i++)
        {
            var displayer = displayers[i];
            if (i < itemsCount)
            {
                displayer.gameObject.SetActive(true);
                displayer.Init(orderData.items[i]);
            }
            else
            {
                displayer.gameObject.SetActive(false);
            }
        }
    }

    public bool IsAtPlace()
    {
        return Processed != null && !Processed.Walker.IsMoving;
    }
}