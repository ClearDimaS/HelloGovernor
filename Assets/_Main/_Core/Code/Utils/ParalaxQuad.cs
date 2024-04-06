using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxQuad : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float size;
    [SerializeField] private Vector3 direction;
    
    [SerializeField] private Transform left;
    [SerializeField] private Transform center;

    private float maxX;
    private float minX;

    private float currentT = 0f;
    private float period = 1f;

    private void Update()
    {
        currentT += Time.deltaTime * speed;
        if (currentT > period)
        {
            currentT -= period;
        }
        ApplyT(currentT);
    }

    private void ApplyT(float t)
    {
        var prevT = t - 1;
        var centerT = t;

        if (prevT > 1f)
        {
            prevT -= period;
        }
        if (centerT > 1f)
        {
            centerT -= period;
        }
        
        left.localPosition = direction * prevT * size;
        center.localPosition = direction * centerT * size;
    }

    public void SetSize(float scaleX)
    {
        size = scaleX;
    }
}
