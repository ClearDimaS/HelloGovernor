using System;
using UnityEngine;
using Zenject;

public class OrderProcessPlace : ProcessPlace
{
    public override float ProcessTime { get; }
    public override bool CanAddProgress(CitizenController citizen)
    {
        throw new NotImplementedException();
    }
}

public abstract class OrderWishGranter : WishGranter<OrderWishGranterConfig, OrderProcessPlace>
{

}