using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class UIWishActivationPlace : CulledBehaviour
{
    [Inject] private PlayerController player;
    [Inject] protected CameraManager cameraManager;
    
    [SerializeField] protected float radius = 1.5f;
    [SerializeField] protected GameObject content;

    private bool isActivated;
    public bool IsShown { get; protected set; }
    public bool IsFinished { get; protected set; }
    private Action onActivate;
    private Func<bool> isFinished;
    private Action onFinish;

    private void Start()
    {
        Hide();
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (IsShown && !isActivated && !cameraManager.IsBlocked)
        {
            var diff = transform.position - player.transform.position;
            diff.y = 0f;
            if (diff.magnitude < radius)
            {
                isActivated = true;
                onActivate?.Invoke();
                content.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
            }
        }

        if (isActivated && !IsFinished && isFinished())
        {
            IsFinished = true;
            onFinish?.Invoke();
            Hide();
        }
    }

    public void Show(Action onActivate, Func<bool> isFinished, Action onFinish)
    {
        this.onActivate = onActivate;
        this.isFinished = isFinished;
        this.onFinish = onFinish;
        
        IsFinished = false;
        isActivated = false;
        gameObject.SetActive(true);
        content.transform.localScale = Vector3.zero;
        content.transform.DOScale(Vector3.one * 1.2f, 0.3f).OnComplete(() =>
        {
            content.transform.DOScale(Vector3.one * 1f, 0.3f);
        });
        IsShown = true;
    }

    public void Hide()
    {
        content.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
        IsShown = false;
    }
}
