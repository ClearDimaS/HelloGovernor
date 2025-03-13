using System;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCallManager : Singleton<UpdateCallManager>
{
    [SerializeField] private List<UpdateableBehaviour> updatables = new List<UpdateableBehaviour>();

    public void AddUpdatable(UpdateableBehaviour updateableBehaviour)
    {
        updatables.Add(updateableBehaviour);
    }
    
    public void RemoveUpdatable(UpdateableBehaviour updateableBehaviour)
    {
        updatables.Remove(updateableBehaviour);
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
            #if UNITY_EDITOR
            updatable.UpdateCall(Time.deltaTime);
            #else
            try
            {
                updatable.UpdateCall(Time.deltaTime);
            }
            catch (Exception e)
            {
                
            }
            #endif
        }
    }

    private void LateUpdate()
    {
        for (var i = 0; i < updatables.Count; i++)
        {
            var updatable = updatables[i];
#if UNITY_EDITOR
            updatable.LateUpdateCall(Time.deltaTime);
#else
            try
            {
            updatable.LateUpdateCall(Time.deltaTime);
            }
            catch (Exception e)
            {
                
            }
#endif
        }
    }
}