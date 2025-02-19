using System;
using UnityEngine;
using Zenject;

public abstract class UIWishGranter : WishGranter<UIWishGranterConfig, UIProcessPlace>
{
    [SerializeField] protected UIWishActivationPlace activationPlace;

    protected virtual bool CanAddToStarted { get; }
    protected UI_Panel panel;
    protected bool isStarted;
    protected bool isFinished;
    public bool WasCompletedAtLeastOnce { get; set; }

    protected override void OnStart()
    {
        panel = GetPanel();
        base.OnStart();
    }

    protected abstract UI_Panel GetPanel();

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
        WasCompletedAtLeastOnce = true;
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
        return base.CanAddOneMore() && ((!isStarted && !isFinished) || CanAddToStarted);
    }

    private bool CanShowActivation()
    {
        return CanShowActivation_Internal() && !isStarted && !isFinished;
    }

    protected virtual bool CanShowActivation_Internal()
    {
        return processed.Count == processPlaces.Length;
    }
    
    protected override bool CanAddProgress(CitizenController citizen)
    {
        return base.CanAddProgress(citizen) && isStarted && isFinished;
    }
    
    protected override void OnLeave(CitizenController citizen)
    {
        
    }

    public Transform GetPlayerPlace()
    {
        return activationPlace.transform;
    }

    public string GetTutorialTitle()
    {
        return config.tutorialTitle;
    }

    public Sprite GetTutorialIcon()
    {
        return config.tutorialIcon;
    }
}