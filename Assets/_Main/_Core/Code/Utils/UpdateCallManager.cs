using System.Collections.Generic;
using UnityEngine;

public abstract class UpdateableBehaviour : MonoBehaviour
{
    private void Awake()
    {
        UpdateCallManager.Instance.AddUpdatable(this);
        OnAwake();
    }

    protected virtual void OnAwake()
    {
        
    }

    public virtual void UpdateCall(float deltaTime)
    {
        
    }

    public virtual void LateUpdateCall(float deltaTime)
    {
        
    }
}

public class UpdateCallManager : Singleton<UpdateCallManager>
{
    private List<UpdateableBehaviour> updatables = new List<UpdateableBehaviour>();

    public void AddUpdatable(UpdateableBehaviour updateableBehaviour)
    {
        updatables.Add(updateableBehaviour);
    }

    private void Update()
    {
        for (var i = 0; i < updatables.Count; i++)
        {
            var updatable = updatables[i];
            if (updatable == null)
            {
                updatables.RemoveAt(i);
                i--;
                continue;
            }
            updatable.UpdateCall(Time.deltaTime);
        }
    }

    private void LateUpdate()
    {
        for (var i = 0; i < updatables.Count; i++)
        {
            var updatable = updatables[i];
            updatable.LateUpdateCall(Time.deltaTime);
        }
    }
}