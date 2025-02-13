using System;

public interface IItemTaker : IRootProvider
{
    public bool CanAddItems(ItemsWishGranter granter);
    public void AddItem(GenericCitizenItem takeItem);
}