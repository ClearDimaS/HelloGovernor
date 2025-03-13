using System;
using UnityEngine;

public class RendererWithSizeCulledRoot : CulledRoot
{
    [SerializeField] private OnVisibilityChangeNotifier visibilityNotifier;
    
    public override bool IsVisible => visibilityNotifier.isVisible;
}