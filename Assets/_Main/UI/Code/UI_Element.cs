using System;
using UnityEngine;

public abstract class UI_Element : MonoBehaviour
{
    private Canvas canvas;
    private UI_ElementAnimator animator;
    public event Action showEvent;
    public event Action shownEvent;

    private bool isShown;
    private bool isInit;
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (isInit)
        {
            return;
        }
        canvas = GetComponent<Canvas>();
        animator = GetComponent<UI_ElementAnimator>();
        OnAwake();
        isInit = true;
    }

    protected virtual void OnAwake()
    {
        
    }

    public void Show()
    {
        isShown = true;
        Init();
        gameObject.SetActive(true);
        OnShow();
        showEvent?.Invoke();
        if (animator == null)
        {
            OnShown();
        }
        else
        {
            animator.Animate(OnShown);
        }
    }

    public void Hide()
    {
        isShown = false;
        OnHide();
        if (animator == null)
        {
            OnHidden();
        }
        else
        {
            animator.AnimateBack(OnHidden);
        }
    }
    
    public bool IsShown()
    {
        return isShown;
    }

    public void SetOrder(int order)
    {
        Init();
        if (canvas == null)
        {
            Debug.LogWarning($"null canvas at: {transform.name}  {GetInstanceID()}");
        }
        //canvas.sortingOrder = order;
        //canvas.overrideSorting = true;
    }

    public virtual void OnShown()
    {
        shownEvent?.Invoke();
    }

    public virtual void OnHidden()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnHide()
    {
        
    }
    
    public virtual void OnShow()
    {
        
    }
}