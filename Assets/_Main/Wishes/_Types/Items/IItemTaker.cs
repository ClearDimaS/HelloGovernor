using System;

public interface IItemTaker : IRootProvider
{
    public bool CanAddItems();
    public void AddItem(GenericCitizenItem takeItem);
}