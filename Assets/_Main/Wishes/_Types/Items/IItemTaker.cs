using System;

public interface IItemTaker : IRootProvider
{
    public bool HasItems(GenericCitizenItem item);
    public bool CanAddItems();
    public void AddItem(GenericCitizenItem takeItem);
}