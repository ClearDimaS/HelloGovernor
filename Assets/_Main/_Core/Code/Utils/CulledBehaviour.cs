using System;
using UnityEngine;

public class CulledBehaviour : MonoBehaviour
{
    private Renderer renderer;

    private bool isVisible;
    
    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            var mr = gameObject.AddComponent<MeshRenderer>();
            mr.materials = Array.Empty<Material>();
            renderer = mr;
        }

        OnAwake();
    }

    private void Update()
    {
        if (isVisible)
        {
            OnUpdate();
        }
    }

    private void LateUpdate()
    {
        if (isVisible)
        {
            OnLateUpdate();
        }
    }

    protected virtual void OnAwake()
    {
        
    }

    protected virtual void OnUpdate()
    {
        
    }
    
    protected virtual void OnLateUpdate()
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