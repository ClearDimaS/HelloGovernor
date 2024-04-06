using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private void LateUpdate()
    {
        var cam = Camera.main.transform;
        transform.rotation = Quaternion.LookRotation(cam.forward, cam.up);
    }
}
