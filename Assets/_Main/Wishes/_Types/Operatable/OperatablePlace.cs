using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class OperatablePlace : CulledBehaviour
{
    [Inject] protected PlayerController player;
    [SerializeField] protected Transform operatedPlace;
    [SerializeField] protected float radius = 1f;
    protected List<IOperator> operators = new ();
    
    public bool IsOperated { get; set; }

    private void Start()
    {
        operators.Add(player);
    }

    public void AddOperator(IOperator @operator)
    {
        operators.Add(@operator);
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        IsOperated = false;
        foreach (var @operator in operators)
        {
            var diff = @operator.Root.position - operatedPlace.position;
            diff.y = 0f;
            if (diff.sqrMagnitude < radius * radius)
            {
                IsOperated = true;
            }
        }
    }

    public Quaternion GetTargetRotation()
    {
        return operatedPlace.rotation;
    }

    public Vector3 GetTargetPosition()
    {
        return operatedPlace.position;
    }
    
    public Transform GetTargetPlaceTransform()
    {
        return operatedPlace;
    }
}