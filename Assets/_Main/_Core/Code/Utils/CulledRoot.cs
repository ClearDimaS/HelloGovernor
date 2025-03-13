using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class CulledRoot : UpdateableBehaviour
{
    public virtual bool IsVisible { get; }
    [SerializeField] protected List<CulledBehaviour> culledBehaviours = new ();

    public override void UpdateCall(float deltaTime)
    {
        base.UpdateCall(deltaTime);
        for (var i = 0; i < culledBehaviours.Count; i++)
        {
            var culled = culledBehaviours[i];
#if UNITY_EDITOR
            culled.UpdateCulled(IsVisible);
#else
            try
            {
               culled.UpdateCulled(IsVisible);
            }
            catch (Exception e)
            {
                
            }
#endif

        }
    }
    
    public override void LateUpdateCall(float deltaTime)
    {
        base.UpdateCall(deltaTime);
        for (var i = 0; i < culledBehaviours.Count; i++)
        {
            var culled = culledBehaviours[i];
#if UNITY_EDITOR
            culled.LateUpdateCulled(IsVisible);
#else
              try
            {
                culled.LateUpdateCulled(IsVisible);
            }
            catch (Exception e)
            {
                
            }
#endif
        }
    }

    public virtual void AddCulledBehaviour(CulledBehaviour culledBehaviour)
    {
        culledBehaviours.Add(culledBehaviour);
    }

    public virtual void RemoveCulledBehaviour(CulledBehaviour culledBehaviour)
    {
        culledBehaviours.Remove(culledBehaviour);
    }
}