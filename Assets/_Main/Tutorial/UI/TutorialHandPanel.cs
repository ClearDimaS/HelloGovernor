using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public enum TutorialHandType
{
    None,
    DoubleTap,
    Drag, 
    Tap
}
public class TutorialHandPanel : UI_Panel
{
    [Inject] private PlayerDataRepository playerRepository;

    [SerializeField] private float dragTime;
    [SerializeField] private float dragPause;
    
    [SerializeField] private RectTransform tapRect;
    [SerializeField] private RectTransform doubleTapRect;
    [SerializeField] private RectTransform dragRect;

    private TutorialHandType handType;
    private Camera cam;

    private Canvas tapRectRootCanvas;
    private RectTransform tapRectTarget;
    private Transform tapTarget;
    private Vector3 worldTapOffset;
    bool isTapRect = false;
    
    private Transform doubleTapTarget;
    private Vector3 worldDoubleTapOffset;

    private Canvas rootCanvas;
    private float dragT;
    private float dragDoneTimer;
    private Transform fromTarget;
    private Transform toTarget;
    private Vector3 worldOffsetFrom;
    private Vector3 worldOffsetTo;


    protected override void OnAwake()
    {
        base.OnAwake();
        rootCanvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        RefreshHandState();
        cam = Camera.main;
    }

    public override void OnShow()
    {
        handType = TutorialHandType.None;
        RefreshHandState();
        base.OnShow();
    }

    private void Update()
    {
        if (handType == TutorialHandType.Drag)
        {
            dragT += Time.deltaTime / dragTime;
            if (dragT > 1f)
            {
                dragDoneTimer += Time.deltaTime;
                if (dragDoneTimer > dragPause)
                {
                    dragT = 0f;
                    dragDoneTimer = 0f;
                }
            }
            
            UpdateDragPosition();
        }

        if (handType == TutorialHandType.DoubleTap)
        {
            UpdateDoubleTapPosition();
        }

        if (handType == TutorialHandType.Tap)
        {
            UpdateTapPosition();
        }
    }

    public void ShowDoubleTap(Transform target, Vector3? worldOffset = null)
    {
        handType = TutorialHandType.DoubleTap;
        doubleTapTarget = target;
        RefreshHandState();
        
        if (worldOffset.HasValue)
        {
            this.worldDoubleTapOffset = worldOffset.Value;   
        }
        else
        {
            this.worldDoubleTapOffset = Vector3.zero;
        }
    }
    
    public void ShowTap(Transform target, Vector3? worldOffset = null)
    {
        isTapRect = false;
        handType = TutorialHandType.Tap;
        tapTarget = target;
        RefreshHandState();
        
        if (worldOffset.HasValue)
        {
            this.worldTapOffset = worldOffset.Value;   
        }
        else
        {
            this.worldTapOffset = Vector3.zero;
        }
    }
    
    public void ShowTapRect(RectTransform target, Vector3? worldOffset = null)
    {
        isTapRect = true;
        handType = TutorialHandType.Tap;
        tapRectTarget = target;
        tapRectRootCanvas = tapRectTarget.GetComponentsInParent<Canvas>().Last();
        RefreshHandState();
        
        if (worldOffset.HasValue)
        {
            this.worldTapOffset = worldOffset.Value;   
        }
        else
        {
            this.worldTapOffset = Vector3.zero;
        }
    }
    
    public void ShowDrag(Transform from, Transform to, Vector3? worldOffsetFrom = null, Vector3? worldOffsetTo = null)
    {
        handType = TutorialHandType.Drag;
        fromTarget = from;
        toTarget = to;
        RefreshHandState();

        dragT = 0f;
        dragDoneTimer = 0f;
        
        if (worldOffsetFrom.HasValue)
        {
            this.worldOffsetFrom = worldOffsetFrom.Value;   
        }
        else
        {
            this.worldOffsetFrom = Vector3.zero;
        }
        
        if (worldOffsetTo.HasValue)
        {
            this.worldOffsetTo = worldOffsetTo.Value;   
        }
        else
        {
            this.worldOffsetTo = Vector3.zero;
        }
    }

    private void UpdateTapPosition()
    {
        var sp = Vector3.zero;
        if (isTapRect)
        {
            var cam = tapRectRootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera;
            sp = RectTransformUtility.WorldToScreenPoint(cam, tapRectTarget.transform.position);
        }
        else
        {
            sp = cam.WorldToScreenPoint(tapTarget.transform.position + worldTapOffset);
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(tapRect.parent.GetComponent<RectTransform>(),
            sp, GetCanvasCamera(), out Vector2 lp);
        tapRect.anchoredPosition = lp;
    }

    private void UpdateDoubleTapPosition()
    {
        var sp = cam.WorldToScreenPoint(doubleTapTarget.transform.position + worldDoubleTapOffset);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(doubleTapRect.parent.GetComponent<RectTransform>(),
            sp, GetCanvasCamera(), out Vector2 lp);
        doubleTapRect.anchoredPosition = lp;
    }
    
    private void UpdateDragPosition()
    {
        var spFrom = cam.WorldToScreenPoint(fromTarget.transform.position + worldOffsetFrom);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(dragRect.parent.GetComponent<RectTransform>(),
            spFrom, GetCanvasCamera(), out Vector2 lpFrom);
        
        var spTo = cam.WorldToScreenPoint(toTarget.transform.position + worldOffsetTo);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(dragRect.parent.GetComponent<RectTransform>(),
            spTo, GetCanvasCamera(), out Vector2 lpTo);
        
        dragRect.anchoredPosition = Vector2.Lerp(lpFrom, lpTo, dragT);
    }

    private Camera GetCanvasCamera()
    {
        return rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera;
    }

    private void RefreshHandState()
    {
        tapRect.gameObject.SetActive(handType == TutorialHandType.Tap);
        doubleTapRect.gameObject.SetActive(handType == TutorialHandType.DoubleTap);
        dragRect.gameObject.SetActive(handType == TutorialHandType.Drag);
    }
}
