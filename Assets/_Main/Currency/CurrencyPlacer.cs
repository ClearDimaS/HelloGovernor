public class CurrencyPlacer : GridPlacer<CurrencyBehaviour>
{
    public CurrencyBehaviour GetLast()
    {
        return placedObjects[placedObjects.Count - 1];
    }

    public int GetMoneyAmount()
    {
        var count = 0;
        foreach (var currency in placedObjects)
        {
            count += currency.Amount;
        }

        return count;
    }
}