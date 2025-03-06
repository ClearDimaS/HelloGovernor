using System;
using UnityEngine;

public class UIGranterTutorialStep : TutorialStep
{
    protected UIWishGranter uiGranter;
    protected bool wasLaunched;
    public UIGranterTutorialStep(UIWishGranter uiGranter, PlayerDataRepository repository) : base(repository)
    {
        this.uiGranter = uiGranter;
    }

    protected override string CreateKey()
    {
        return $"uigranter_{uiGranter.GetType().Name}";
    }

    protected override void UpdateProgress_Internal()
    {
        if (!uiGranter.IsCooldown && !wasLaunched)
        {
            wasLaunched = true;
        }
        if (uiGranter.IsCooldown && !IsCompleted() && !wasLaunched)
        {
            uiGranter.SetReady();
        }
    }

    public override float GetProgress()
    {
        return uiGranter.WasCompletedAtLeastOnce ? 1f : 0f;
    }

    public override Transform GetCameraTarget()
    {
        return uiGranter.GetPlayerPlace();
    }

    public override Transform GetArrowTarget()
    {
        return uiGranter.GetPlayerPlace();
    }

    protected override string CreateProgressText()
    {
        return uiGranter.WasCompletedAtLeastOnce ? "1/1" : "0/1";
    }

    protected override string CreateTitle()
    {
        return $"{uiGranter.GetTutorialTitle()}";
    }

    public override Sprite GetTutorialIcon()
    {
        return uiGranter.GetTutorialIcon();
    }
}