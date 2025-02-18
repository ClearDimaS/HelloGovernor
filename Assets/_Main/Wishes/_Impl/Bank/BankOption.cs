using System;

[Serializable]
public class BankOption
{
    public EBankOption type;
    public int x;
    public int y;

    public bool IsSelected { get; private set; }
    public BankOption(EBankOption type, int x, int y, string text)
    {
        this.type = type;
        this.x = x;
        this.y = y;
    }

    public void Select()
    {
        IsSelected = true;
    }

    public int GetReward()
    {
        switch (type)
        {
            case EBankOption.Add:
                return x + y;
            case EBankOption.Subtract:
                return x - y;
            case EBankOption.Multiply:
                return x * y;
            case EBankOption.Divide:
                return x / y;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}