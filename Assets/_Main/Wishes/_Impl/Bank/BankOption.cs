using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class BankOption
{
    public EBankOption type;
    public int x;
    public int y;

    public BankOption(EBankOption type, int x, int y)
    {
        this.type = type;
        this.x = x;
        this.y = y;
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
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static BankOption CreateFromType(EBankOption newType, int newReward)
    {
        switch (newType)
        {
            case EBankOption.Add:
                var xAdd = Random.Range(1, Mathf.Abs(newReward)/2);
                var yAdd = newReward - xAdd;
                return new BankOption(newType, xAdd, yAdd);
            case EBankOption.Subtract:
                var xSubtract = Random.Range(1, Mathf.Abs(newReward));
                var ySubtract = newReward - xSubtract;
                return new BankOption(newType, xSubtract, ySubtract);
            case EBankOption.Multiply:
                var xMult = Random.Range(3, Mathf.Abs(newReward)/2);
                var yMult = newReward/xMult;
                return new BankOption(newType, xMult, yMult);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}