using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimplePhysicsBehaviour<T> : SimplePhysicsBehaviourBase<T> where T : Component
{
    protected override bool IsInside(T component)
    {
        var diff = transform.position - component.transform.position;
        diff.y = 0;
        var sqrDist = diff.sqrMagnitude;
        return sqrRadius > sqrDist;
    }
}

public interface IRootProvider
{
    public Transform Root { get; }
}
public abstract class SimplePhysicsBehaviourInterface<T> : SimplePhysicsBehaviourBase<T> where T : IRootProvider
{
    protected override bool IsInside(T component)
    {
        var diff = transform.position - component.Root.position;
        diff.y = 0;
        var sqrDist = diff.sqrMagnitude;
        return sqrRadius > sqrDist;
    }
}

public abstract class SimplePhysicsBehaviourBase<T> : CulledBehaviour
{
    [SerializeField] private bool ignoreVisible;
    [SerializeField] private float interactRadius = 1.3f;

    private List<T> components;
    private List<int> isInsideStatuses = new ();
    
    protected float sqrRadius;

    protected override void OnAwake()
    {
        base.OnAwake();
        sqrRadius = interactRadius * interactRadius;
        components = GetComponentsForWork();
        foreach (var component in components)
        {
            isInsideStatuses.Add(-1);
        }
    }

    protected abstract List<T> GetComponentsForWork();

    protected void AddComponentForWork(T component)
    {
        components.Add(component);
        isInsideStatuses.Add(-1);
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (visible || ignoreVisible)
        {
            for (var i = 0; i < components.Count; i++)
            {
                var component  = components[i];
                var isInside = isInsideStatuses[i];

                var isInsideNew = IsInside(component);

                if (isInside == 1)
                {
                    if (!isInsideNew)
                    {
                        OnLeave(component);
                        isInsideStatuses[i] = 0;
                    }
                }
                else
                {
                    if (isInsideNew)
                    {
                        OnEnter(component);
                        isInsideStatuses[i] = 1;
                    }
                }
            }
        }
    }

    protected abstract bool IsInside(T component);

    protected virtual void OnEnter(T component)
    {

    }

    protected virtual void OnLeave(T component)
    {
 
    }
}

public class CulledBehaviour : UpdateableBehaviour
{
    private Renderer renderer;

    private bool isVisible;
    public bool IsVisible => isVisible;

    protected override void OnAwake()
    {
        base.OnAwake();
        renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            var mr = gameObject.AddComponent<MeshRenderer>();
            mr.materials = Array.Empty<Material>();
            renderer = mr;
        }
    }

    public override void UpdateCall(float deltaTime)
    {
        base.UpdateCall(deltaTime);
        OnUpdate(isVisible);
    }

    public override void LateUpdateCall(float deltaTime)
    {
        base.LateUpdateCall(deltaTime);
        OnLateUpdate(isVisible);
    }

    protected virtual void OnUpdate(bool visible)
    {
        
    }
    
    protected virtual void OnLateUpdate(bool visible)
    {
        
    }

    private void OnBecameVisible()
    {
        isVisible = true;
    }
    
    private void OnBecameInvisible()
    {
        isVisible = false;
    }
}