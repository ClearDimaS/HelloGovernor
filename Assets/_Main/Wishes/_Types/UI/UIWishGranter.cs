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

    protected override bool IsPlayerProcessing(CitizenController citizenController)
    {
        return false;
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
            ShowActivationPlace();
        }

        if (isStarted && !panel.IsShown())
        {
            isFinished = true;
            HideActivationPlace();
        }
    }

    protected virtual void ShowActivationPlace()
    {
        activationPlace.Show(OnActivate, IsFinished, OnFinish);
    }

    protected virtual void HideActivationPlace()
    {
        activationPlace.Hide();
    }

    protected override void OnPurchase()
    {
        SetReady();
        base.OnPurchase();
    }

    protected virtual void OnResetActivation()
    {
        WasCompletedAtLeastOnce = true;
        LeaveAllCitizens();
    }

    protected virtual void OnActivate()
    {
        panel.Show();
        var gameTimer = coolDownTimer.GetGameTimer();
        if (gameTimer != null)
        {
            gameTimer.SetStarted();
        }
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
        return processed.Count > 0 && !coolDownTimer.IsCooldown;
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