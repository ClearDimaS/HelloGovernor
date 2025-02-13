using System;
using UnityEngine;
using Zenject;

public abstract class UIWishGranter<T> : WishGranter<UIWishGranterConfig, UIProcessPlace> where T : UI_Panel
{
    [Inject] protected UI_Manager uiManager;
    
    [SerializeField] protected UIWishActivationPlace activationPlace;

    protected UI_Panel panel;
    protected bool isStarted;
    protected bool isFinished;
    
    private void Start()
    {
        panel = uiManager.GetPanel<T>();
        OnStart();
    }

    protected virtual void OnStart()
    {
        
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (isStarted && isFinished && processed.Count == 0)
        {
            isStarted = false;
            isFinished = false;
            OnResetActivation();
        }
        if (!activationPlace.IsShown && CanShowActivation())
        {
            activationPlace.Show(OnActivate, IsFinished, OnFinish);
        }

        if (isStarted && !panel.IsShown())
        {
            isFinished = true;
            activationPlace.Hide();
        }
    }

    protected virtual void OnResetActivation()
    {
    
    }

    protected virtual void OnActivate()
    {
        panel.Show();
        isStarted = true;
    }

    protected virtual bool IsFinished()
    {
        return isFinished;
    }

    protected virtual void OnFinish()
    {
        panel.Hide();
    }

    public override bool CanAddOneMore()
    {
        return base.CanAddOneMore() && !isStarted && !isFinished;
    }

    protected virtual bool CanShowActivation()
    {
        return processed.Count == processPlaces.Length && !isStarted && !isFinished;
    }

    protected override bool CanAddProgress(CitizenController citizen)
    {
        return base.CanAddProgress(citizen) && isStarted && isFinished;
    }
    
    protected override void OnLeave(CitizenController citizen)
    {
        
    }
}