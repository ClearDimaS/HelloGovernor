using System;
using UnityEngine;

public class OperatableTutorialStep : TutorialStep
{
    protected CameraManager cameraManager;
    protected OperatableGranter operatable;
    protected float targetServeCount = 3f;

    protected bool wasServeShown;
    protected Transform startTarget;
    public OperatableTutorialStep(OperatableGranter operatable,CameraManager cameraManager, PlayerDataRepository repository, 
        TutorialsConfig tutorialsConfig) : base(repository, tutorialsConfig)
    {
        this.operatable = operatable;
        this.cameraManager = cameraManager;
    }

    protected override string CreateKey()
    {
        return $"learn_{operatable.GetType().Name}";
    }

    protected override void UpdateProgress_Internal()
    {
        if (!IsCompleted())
        {
            if (startTarget == null)
            {
                startTarget = GetArrowTarget();
            }
            if (!wasServeShown && startTarget != GetArrowTarget() && startTarget != null)
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
        return string.Format(tutorialsConfig.OperatableTitle, Mathf.RoundToInt(targetServeCount));
    }

    public override Sprite GetTutorialIcon()
    {
        return operatable.GetIconOperator();
    }
}