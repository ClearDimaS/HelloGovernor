using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class SkinPreviewPanel : UI_Panel
{
    [Inject] private SkinChooser skinChooser;
    [Inject] private PlayerDataRepository playerRepository;

    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button selectButton;
    [SerializeField] private Image selectedImg;
    [SerializeField] private Image notSelectedImg;
    [SerializeField] private EventTrigger trigger;

    [SerializeField] private SwiperNextPreviousButtons swiperNextPreviousButtons;

    private int lastIndex = -1;
    private int lastPrice;
    
    public event Action saveEvent;
    
    public event Action pointerDownEvent;
    public event Action dragEvent;
    public event Action pointerUpEvent;

    protected override void OnAwake()
    {
        base.OnAwake();
        buyButton.onClick.AddListener(Buy);
        closeButton.onClick.AddListener(Close);
        selectButton.onClick.AddListener(Select);
        
        var downEntry = new EventTrigger.Entry();
        downEntry.callback.AddListener(_ => pointerDownEvent?.Invoke());
        downEntry.eventID = EventTriggerType.PointerDown;
        
        var dragEntry = new EventTrigger.Entry();
        dragEntry.callback.AddListener(_ => dragEvent?.Invoke());
        dragEntry.eventID = EventTriggerType.Drag;
        
        var upEntry = new EventTrigger.Entry();
        upEntry.callback.AddListener(_ => pointerUpEvent?.Invoke());
        upEntry.eventID = EventTriggerType.PointerUp;
        
        trigger.triggers.Add(downEntry);
        trigger.triggers.Add(dragEntry);
        trigger.triggers.Add(upEntry);
    }

    private void Update()
    {
        if (lastIndex != skinChooser.SkinIndex)
        {
            lastIndex = skinChooser.SkinIndex;
            RefreshState();
        }
    }

    private void RefreshState()
    {
        var canBuy = skinChooser.CurrentPrice <= playerRepository.Money;
        var isBought = playerRepository.IsSkinBought(lastIndex);
        var isSelected = playerRepository.SkinIndex == lastIndex;

        buyButton.UpdateState(!isBought);
        selectButton.UpdateState(isBought);
        if (isBought)
        {
            selectedImg.UpdateState(isSelected);
            notSelectedImg.UpdateState(!isSelected);
        }
        if (buyButton.interactable != canBuy)
        {
            buyButton.interactable = canBuy;
        }
        if (lastPrice != skinChooser.CurrentPrice)
        {
            lastPrice = skinChooser.CurrentPrice;
            priceText.color = playerRepository.Money >= lastPrice ? Color.white : Color.red;
            priceText.text = lastPrice.ToString();
        }
    }

    private void Buy()
    {
        if (!playerRepository.IsSkinBought(skinChooser.SkinIndex) && playerRepository.Money >= skinChooser.CurrentPrice)
        {
            playerRepository.Money -= skinChooser.CurrentPrice;
            playerRepository.SetSkinBought(skinChooser.SkinIndex);
        }
        Select();
    }

    private void Select()
    {
        if (playerRepository.IsSkinBought(skinChooser.SkinIndex))
        {
            playerRepository.SkinIndex = skinChooser.SkinIndex;
            RefreshState();
        }
    }

    public void SetSwiper(ElementsSwiper swiper)
    {
        swiperNextPreviousButtons.SetSwiper(swiper);
    }
    
    private void Close()
    {
        saveEvent?.Invoke();
        Hide();
    }

    private Vector3 originalPos;
    public override void OnShow()
    {
        base.OnShow();
        UI_Manager.Instance.ClosePanel<JoystickPanel>();
        originalPos = PlayerController.Instance.transform.position;
    }

    public override void OnShown()
    {
        base.OnShown();
        PlayerController.Instance.transform.position += new Vector3(1000f, 0, 1000f);
    }

    public override void OnHide()
    {
        base.OnHide();
        UI_Manager.Instance.OpenPanel<JoystickPanel>();
        PlayerController.Instance.transform.position = originalPos + Vector3.forward * 2;
    }
}
