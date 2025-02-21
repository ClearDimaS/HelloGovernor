using System;
using System.Collections.Generic;

[Serializable]
public class OrderData
{
    public List<OrderItemsConfigData> items = new ();

    public OrderData(params OrderItemsConfigData[] newItems)
    {
        Reinit(newItems);
    }

    public void Reinit(params OrderItemsConfigData[] newItems)
    {
        items.Clear();
        foreach (var item in newItems)
        {
            items.Add(item);
        }
    }

    public void AddItem(OrderItemsConfigData item)
    {
        items.Add(item);
    }
}