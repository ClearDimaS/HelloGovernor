using System;
using UnityEngine;

public class OperatableProcessPlace : ProcessPlace
{
    [SerializeField] protected TimerBase timer;
    private OperatableGranter granter;
    
    public override float ProcessTime { get; }
    
    private void Start()
    {
        granter = GetComponentInParent<OperatableGranter>();
        timer.SetIcon(granter.GetIconOperator());
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        timer.SetProgress(progress);
    }

    public override bool CanAddProgress(CitizenController citizen)
    {
        return granter.IsOperated();
    }
}