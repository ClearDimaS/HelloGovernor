using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FashionBuilding : SimplePlayerPhysicsBehaviour
{
    [SerializeField] private SkinChooser skinChooser;

    protected override void OnEnter(PlayerController component)
    {
        base.OnEnter(component);
        skinChooser.Show();
    }
}
