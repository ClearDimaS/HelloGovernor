using System.Collections.Generic;
using UnityEngine;

public abstract class OperatablePlace : CulledBehaviour
{
    [SerializeField] protected Transform operatedPlace;
    [SerializeField] protected float radius = 1f;
    protected List<IOperator> operators = new ();
    
    public bool IsOperated { get; set; }
    
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
            if (diff.sqrMagnitude < radius * radius)
            {
                IsOperated = true;
            }
        }
    }
}