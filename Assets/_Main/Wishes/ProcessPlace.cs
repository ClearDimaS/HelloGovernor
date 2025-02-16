using System;
using UnityEngine;

public class SimpleProcessPlace : ProcessPlace
{
    protected WishGranter granter;
    public override float ProcessTime => granter.FullProgressTime;

    private void Start()
    {
        granter = GetComponentInParent<WishGranter>();
    }

    public override bool CanAddProgress(CitizenController citizen)
    {
        return true;
    }
}

public abstract class ProcessPlace : CulledBehaviour
{
    private CitizenController processed;
    public Vector3 Position => transform.position;
    
    public abstract float ProcessTime { get; }
    protected float progress;

    public abstract bool CanAddProgress(CitizenController citizen);

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (processed != null && CanAddProgress(processed))
        {
            progress += Time.deltaTime / Mathf.Max(ProcessTime, 0.001f);
        }
        else
        {
            progress = 0f;
        }
    }

    public void SetOwner(CitizenController citizen)
    {
        if (processed != null)
        {
            throw new NotImplementedException("setting owner of busy place");
        }
        processed = citizen;
    }
    
    public CitizenController GetOwner()
    {
        return processed;
    }
    
    public void LeavePlace(CitizenController citizen)
    {
        if (processed != citizen)
        {
            throw new NotImplementedException("leaving place which doesnt belong to me");
        }

        progress = 0f;
        processed = null;
    }

    public Transform GetTargetTransform()
    {
        return transform;
    }
}