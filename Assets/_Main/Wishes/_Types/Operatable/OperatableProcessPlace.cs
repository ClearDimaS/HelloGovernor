using System;
using UnityEngine;

public class OperatableProcessPlace : ProcessPlace
{
    [SerializeField] protected TimerBase timer;
    private OperatableGranter granter;

    
    private void Start()
    {
        granter = GetComponentInParent<OperatableGranter>();
        timer.SetIcon(granter.GetIconTimer());
        var operatedPos = granter.GetOperatedPlace().transform.position;
        timer.transform.position = new Vector3(operatedPos.x, timer.transform.position.y, operatedPos.z);
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        var showTimer = ShowTimer();
        
        if (showTimer != timer.gameObject.activeSelf)
        {
            timer.gameObject.SetActive(showTimer);
        }
        if (showTimer)
        {
            timer.SetProgress(progress);
        }
    }

    public override bool CanAddProgress(CitizenController citizen)
    {
        return granter.IsOperated();
    }
}