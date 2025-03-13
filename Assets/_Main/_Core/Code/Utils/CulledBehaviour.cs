using System.Collections.Generic;
using UnityEngine;

public abstract class SimplePhysicsBehaviour<T> : SimplePhysicsBehaviourBase<T> where T : Component
{
    protected override bool IsInside(T component)
    {
        if (!component.gameObject.activeInHierarchy)
        {
            return false;
        }
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
    public abstract Transform Center { get; }
    protected override bool IsInside(T component)
    {
        if (!component.Root.gameObject.activeInHierarchy)
        {
            return false;
        }
        var diff = Center.position - component.Root.position;
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
        else
        {
            for (var i = 0; i < components.Count; i++)
            {
                isInsideStatuses[i] = 0;
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

public class CulledBehaviour : MonoBehaviour
{
    private CulledRoot culledRoot;
    
    public bool IsVisible => culledRoot.IsVisible;

    protected void Awake()
    {
        culledRoot = GetComponentInParent<CulledRoot>();
        if (culledRoot != null)
        {
            culledRoot.AddCulledBehaviour(this);
        }
        else
        {
            Debug.LogError($"culled root is null for {GetType()} at {transform.GetHierarchyString()}");
        }

        OnAwake();
    }

    protected virtual void OnAwake()
    {
        
    }
    
    public void UpdateCulled(bool isVisible)
    {
        OnUpdate(isVisible);
    }

    public void LateUpdateCulled(bool isVisible)
    {
        OnLateUpdate(isVisible);
    }

    protected virtual void OnUpdate(bool visible)
    {
        
    }
    
    protected virtual void OnLateUpdate(bool visible)
    {
        
    }
}