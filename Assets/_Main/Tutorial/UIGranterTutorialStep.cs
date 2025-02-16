using System;
using UnityEngine;

public class UIGranterTutorialStep : TutorialStep
{
    protected UIWishGranter uiGranter;
    
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
        
    }

    public override float GetProgress()
    {
        return uiGranter.WasCompletedAtLeastOnce ? 0f : 1f;
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
        return uiGranter.WasCompletedAtLeastOnce ? "0/1" : "1/1";
    }

    protected override string CreateTitle()
    {
        return $"{uiGranter.GetTutorialTitle()}";
    }
}