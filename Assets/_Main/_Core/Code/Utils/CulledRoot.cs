using System.Collections.Generic;
using UnityEngine;

public abstract class CulledRoot : UpdateableBehaviour
{
    public virtual bool IsVisible { get; }
    [SerializeField] protected List<CulledBehaviour> culledBehaviours = new ();

    public override void UpdateCall(float deltaTime)
    {
        base.UpdateCall(deltaTime);
        foreach (var culled in culledBehaviours)
        {
            culled.UpdateCulled(IsVisible);
        }
    }
    
    public override void LateUpdateCall(float deltaTime)
    {
        base.UpdateCall(deltaTime);
        foreach (var culled in culledBehaviours)
        {
            culled.LateUpdateCulled(IsVisible);
        }
    }

    public void AddCulledBehaviour(CulledBehaviour culledBehaviour)
    {
        culledBehaviours.Add(culledBehaviour);
    }
}