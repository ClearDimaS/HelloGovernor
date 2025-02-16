using System;
using UnityEngine;

public class OperatableTutorialStep : TutorialStep
{
    protected OperatableGranter operatable;
    protected float targetServeCount = 3f;
    
    public OperatableTutorialStep(OperatableGranter operatable, PlayerDataRepository repository) : base(repository)
    {
        this.operatable = operatable;
    }

    protected override string CreateKey()
    {
        return $"learn_{operatable.GetType().Name}";
    }

    protected override void UpdateProgress_Internal()
    {
        
    }

    public override float GetProgress()
    {
        return operatable.ServedCounter / targetServeCount;
    }

    public override Transform GetCameraTarget()
    {
        return operatable.GetOperatedPlace().GetTargetPlaceTransform();
    }

    public override Transform GetArrowTarget()
    {
        return operatable.GetOperatedPlace().GetTargetPlaceTransform();
    }

    protected override string CreateProgressText()
    {
        return $"{operatable.ServedCounter}/{Mathf.RoundToInt(targetServeCount)}";
    }

    protected override string CreateTitle()
    {
        return $"Serve {Mathf.RoundToInt(targetServeCount)} people";
    }

    public override Sprite GetTutorialIcon()
    {
        return operatable.GetIconOperator();
    }
}