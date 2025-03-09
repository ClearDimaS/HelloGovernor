using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BankOptionUIElement : MonoBehaviour
{
    [SerializeField] private TMP_Text optionTextArg1;
    [SerializeField] private GameObject multiplyOp;
    [SerializeField] private GameObject addOp;
    [SerializeField] private GameObject subtractOp;
    [SerializeField] private TMP_Text optionTextArg2;
    [SerializeField] private Button optionButton;

    [SerializeField] private Color color1;
    [SerializeField] private Color color2;

    public Color ColorRight => color1;
    public Color ColorWrong => color2;
    private Action onSelect;
    private BankOption option;
    
    private void Awake()
    {
        optionButton.onClick.AddListener(SelectOption);
    }

    public void Init(BankOption option, Action onSelect, bool isPositive)
    {
        this.option = option;
        optionTextArg1.text = option.x.ToString();
        multiplyOp.SetActive(option.type == EBankOption.Multiply);
        addOp.SetActive(option.type == EBankOption.Add);
        subtractOp.SetActive(option.type == EBankOption.Subtract);
        optionTextArg2.text = option.y.ToString();
        //SetColor(isPositive ? color1 : color2);
        
        this.onSelect = onSelect;
    }

    private void SetColor(Color color)
    {
        //optionTextArg1.color = color;
        //optionTextArg2.color = color;
    }

    private void SelectOption()
    {
        onSelect?.Invoke();
    }

    private string GetOpText(BankOption option)
    {
        var x = option.x;
        var y = option.y;
        var type = option.type;
        
        switch (type)
        {
            case EBankOption.Add:
                return "+";
            case EBankOption.Subtract:
                return "-";
            case EBankOption.Multiply:
                return "x";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}