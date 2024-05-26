using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : CulledBehaviour
{
    protected override void OnLateUpdate()
    {
        base.OnLateUpdate();
        var cam = Camera.main.transform;
        transform.rotation = Quaternion.LookRotation(cam.forward, cam.up);
    }
}
