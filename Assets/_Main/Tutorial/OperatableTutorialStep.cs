using System;
using UnityEngine;

public class OperatableTutorialStep : TutorialStep
{
    protected CameraManager cameraManager;
    protected OperatableGranter operatable;
    protected float targetServeCount = 3f;

    protected bool wasServeShown;
    protected Transform startTarget;
    public OperatableTutorialStep(OperatableGranter operatable, PlayerDataRepository repository, CameraManager cameraManager) : base(repository)
    {
        this.operatable = operatable;
        this.cameraManager = cameraManager;
        startTarget = GetArrowTarget();
    }

    protected override string CreateKey()
    {
        return $"learn_{operatable.GetType().Name}";
    }

    protected override void UpdateProgress_Internal()
    {
        if (!IsCompleted())
        {
            if (!wasServeShown && startTarget != GetArrowTarget())
            {
                wasServeShown = true;
                cameraManager.SetTarget(GetCameraTarget(), 2f, distanceMult: 1f, startCallback:() => {});
            }
        }
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