using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SkinPreviewPanel : UI_Panel
{
    [SerializeField] private EventTrigger trigger;
    [SerializeField] private Button closeButton;
    [SerializeField] private SwiperNextPreviousButtons swiperNextPreviousButtons;

    public event Action saveEvent;
    
    public event Action pointerDownEvent;
    public event Action dragEvent;
    public event Action pointerUpEvent;

    protected override void OnAwake()
    {
        base.OnAwake();
        closeButton.onClick.AddListener(Close);
        
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
    
    public void SetSwiper(ElementsSwiper swiper)
    {
        swiperNextPreviousButtons.SetSwiper(swiper);
    }
    
    private void Close()
    {
        saveEvent?.Invoke();
        Hide();
    }
}
