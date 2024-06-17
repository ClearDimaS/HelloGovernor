using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PriceDisplayer : CulledBehaviour
{
    private const float BASE_SIZE_FACTOR = 0.5f;

    [SerializeField] private float scaleOnChange = 1.1f;
    [SerializeField] private GameObject animateContent;
    [SerializeField] private TMP_Text text;
    [SerializeField] private SpriteRenderer itemIcon;
    [SerializeField] private GameObject upgradeGO;
    
    private UpgradableObject upgradableObject;
    private MoneyConsumer moneyConsumer;
    private int lastAmount;

    protected override void OnAwake()
    {
        base.OnAwake();
        moneyConsumer = GetComponentInParent<MoneyConsumer>();
        upgradableObject = GetComponentInParent<UpgradableObject>();
    }

    private void Start()
    {
        RefreshDisplay(moneyConsumer.GetLeftAmount(), false);
        var sprite = upgradableObject.GetItemIcon();
        itemIcon.sprite = sprite;
        
        var pixelsPerUnit = sprite.rect.width / sprite.bounds.size.x;
        itemIcon.transform.localScale = new Vector3(pixelsPerUnit, pixelsPerUnit, pixelsPerUnit) * BASE_SIZE_FACTOR / sprite.rect.width;  ;
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (visible)
        {
            var isUpgrade = upgradableObject.Level > 0;
            if (upgradeGO.activeSelf != isUpgrade)
            {
                upgradeGO.SetActive(isUpgrade);
            }
        
            var amountLeft = moneyConsumer.GetLeftAmount();
            if (lastAmount != amountLeft)
            {
                RefreshDisplay(amountLeft);
                lastAmount = amountLeft;
            }
        }
    }

    private void RefreshDisplay(int amount, bool animate = true)
    {
        text.text = Price.ToMoneyString(amount);
        if (animate)
        {
            animateContent.transform.DOKill();
            animateContent.transform.DOScale(Vector3.one * scaleOnChange, 0.3f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                animateContent.transform.DOScale(Vector3.one * 1f, 0.3f).SetEase(Ease.InCubic);
            });
        }
    }
}
