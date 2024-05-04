using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Zenject;

public class WalletPanel : UI_Panel
{
    [Inject]
    private PlayerDataRepository repository;
    
    [SerializeField] private TMP_Text moneyText;

    private int lastMoney = -99;

    private void Start()
    {
        lastMoney = repository.GetData().money;
        moneyText.text = Price.ToMoneyString(lastMoney);
    }

    private void Update()
    {
        if (repository.GetData().money != lastMoney)
        {
            lastMoney = repository.GetData().money;
            var money = lastMoney;
            moneyText.transform.DOScale(Vector3.one * 1.15f, 0.3f).OnComplete(() =>
            {
                moneyText.text = Price.ToMoneyString(money);
                moneyText.transform.DOScale(Vector3.one, 0.3f);
            });
        }
    }
}
