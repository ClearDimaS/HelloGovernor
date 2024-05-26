using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotate : CulledBehaviour
{
    [SerializeField] private Vector3 axis = new Vector3(0, 1, 0);
    [SerializeField] private float speed = 10;

    protected override void OnUpdate()
    {
        base.OnUpdate();
        transform.localRotation *= Quaternion.AngleAxis(speed * Time.deltaTime, axis);
    }
}
