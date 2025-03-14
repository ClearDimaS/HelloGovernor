using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : CulledBehaviour
{
    protected Camera cam;
    protected override void OnAwake()
    {
        base.OnAwake();
        cam = Camera.main;
    }

    protected override void OnLateUpdate(bool visible)
    {
        base.OnLateUpdate(visible);
        if (visible)
        {
            var diff = transform.position - cam.transform.position;
            transform.rotation = Quaternion.LookRotation(diff.normalized, cam.transform.up);   
        }
    }
}
