using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class OperatablePlace : CulledBehaviour
{
    [Inject] protected PlayerController player;
    [SerializeField] protected Transform operatedPlace;
    [SerializeField] protected float radius = 1f;
    [SerializeField] public OperatableFillPlace fillPlace;
    
    protected List<IOperator> operators = new ();

    public bool IsOperated { get; set; }
    private Func<bool> extraShowCondition;

    private void Start()
    {
        operators.Add(player);
        if (fillPlace != null)
        {
            fillPlace.Add(player);
        }
    }

    public void AddOperator(IOperator @operator)
    {
        operators.Add(@operator);
        if (fillPlace != null)
        {
            fillPlace.Add(@operator);
        }
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        var show = (fillPlace == null || !fillPlace.IsEmpty) && (extraShowCondition == null || extraShowCondition.Invoke());
        if (show != operatedPlace.gameObject.activeSelf)
        {
            operatedPlace.gameObject.SetActive(show);
        }
        IsOperated = false;
        if (show)
        {
            foreach (var @operator in operators)
            {
                if(IsOperatedBy(@operator.Root))
                {
                    IsOperated = true;
                }
            }   
        }
    }

    protected bool IsOperatedBy(Transform t)
    {
        var diff = t.position - operatedPlace.position;
        diff.y = 0f;
        if (diff.sqrMagnitude < radius * radius)
        {
            return true;
        }

        return false;
    }

    public Quaternion GetTargetRotation()
    {
        return operatedPlace.rotation;
    }

    public Vector3 GetTargetPosition()
    {
        if (fillPlace != null && fillPlace.IsEmpty)
        {
            return fillPlace.GetPlace().position;
        }
        return operatedPlace.position;
    }
    
    public Transform GetTargetPlaceTransform()
    {
        if (fillPlace != null && fillPlace.CanFill && fillPlace.IsEmpty)
        {
            return fillPlace.GetPlace();
        }
        return operatedPlace;
    }

    public void SetShowExtraCondition(Func<bool> func)
    {
        extraShowCondition = func;
    }

    public bool IsAnyOperatedByPlayer()
    {
        return IsOperatedBy(player.transform) || (fillPlace != null && fillPlace.IsFilledByPlayer());
    }

    public bool HasEnoughItemsToWork()
    {
        if (fillPlace != null)
        {
            return !fillPlace.IsEmpty && fillPlace.Max > 0;
        }
        return true;
    }
}