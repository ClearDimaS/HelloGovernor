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
            Debug.Log($"updating: {updatable.transform.name}");
            #endif
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