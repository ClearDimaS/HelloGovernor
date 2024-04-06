using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SkyboxSetter : MonoBehaviour
{
    [Serializable]
    public class AngleAxis
    {
        public float angle;
        public float scale;
        public Vector3 axis;
    }

    [SerializeField] private AngleAxis[] rotations;
    [SerializeField] private Material targetMaterial;

    private void Start()
    {
        var m = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
        targetMaterial.SetMatrix("_Rotation", m);
    }
    
    /*void Update()
    {
        var rot = Quaternion.identity;
        var scale = Vector3.zero;
        foreach (var rotationData in rotations)
        {
            scale += rotationData.axis * rotationData.scale;
            rot *= Quaternion.AngleAxis(rotationData.angle, rotationData.axis);
        }

        Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, rot, scale);
        targetMaterial.SetMatrix("_Rotation", m);
    }*/
}
