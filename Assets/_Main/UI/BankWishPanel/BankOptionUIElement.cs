using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BankOptionUIElement : MonoBehaviour
{
    [SerializeField] private TMP_Text optionText;
    [SerializeField] private Button optionButton;

    private Action onSelect;
    private BankOption option;
    
    private void Awake()
    {
        optionButton.onClick.AddListener(SelectOption);
    }

    public void Init(BankOption option, Action onSelect)
    {
        this.option = option;
        optionText.text = CreateText(option);
        this.onSelect = onSelect;
    }

    private void SelectOption()
    {
        option.Select();
        onSelect?.Invoke();
    }

    private string CreateText(BankOption option)
    {
        var x = option.x;
        var y = option.y;
        var type = option.type;
        
        switch (type)
        {
            case EBankOption.Add:
                return $"{x}+{y}";
            case EBankOption.Subtract:
                return $"{x}-{y}";
            case EBankOption.Multiply:
                return $"{x}*{y}";
            case EBankOption.Divide:
                return $"{x}/{y}";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}