using System;
using UnityEngine;

public class RendererCulledRoot : CulledRoot
{
    [SerializeField] private Renderer renderer;
    private bool isVisible;

    public override bool IsVisible => isVisible;

    protected override void OnAwake()
    {
        base.OnAwake();
        if (renderer == null)
        {
            renderer = GetComponent<Renderer>();
        }
        if (renderer == null)
        {
            var mr = gameObject.AddComponent<MeshRenderer>();
            mr.materials = Array.Empty<Material>();
            renderer = mr;
        }
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