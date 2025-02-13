using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class OperatorAssistant : MonoBehaviour, IOperator
{
    [SerializeField] private Walker walker;
    private OperatableGranter wishGranter;
    
    public Transform Root => transform;
    
    private void Awake()
    {
        wishGranter = GetComponentInParent<OperatableGranter>();
    }

    private void Start()
    {
        var operated = wishGranter.GetOperatedPlace();
        operated.AddOperator(this);
    }

    private void Update()
    {
        if (!wishGranter.IsOperated())
        {
            var operated = wishGranter.GetOperatedPlace();
            var place = operated.GetTargetPosition();
            var diff = place - transform.position;
            diff.y = 0f;
            if (diff.magnitude > 0.1f)
            {
                if (!walker.IsMoving)
                {
                    walker.MoveToTarget(place, () =>
                    {
                        walker.transform.DORotateQuaternion(operated.GetTargetRotation(), 0.3f);
                    });      
                }
            }
        }
    }
}
