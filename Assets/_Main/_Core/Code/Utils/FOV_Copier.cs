using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOV_Copier : MonoBehaviour
{
    [SerializeField] private Camera from;
    [SerializeField] private Camera to;

    private void Update()
    {
        to.fieldOfView = from.fieldOfView;
        to.orthographicSize = from.orthographicSize;
        to.orthographic = from.orthographic;
        
        to.transform.position = from.transform.position;
        to.transform.rotation = from.transform.rotation;
    }
}
