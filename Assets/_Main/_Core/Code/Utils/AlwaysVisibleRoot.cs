using System;

public class AlwaysVisibleRoot : CulledRoot
{
    public override bool IsVisible => true;
}