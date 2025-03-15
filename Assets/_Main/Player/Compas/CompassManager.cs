using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ECompasTarget
{
    Thief,
    Bank,
    PressConference,
    Money,
}
public class CompassManager : MonoBehaviour
{
    private CompassController[] compasses;

    private Dictionary<ECompasTarget, CompassController> compassesDict;

    private void Awake()
    {
        compasses = GetComponentsInChildren<CompassController>();
        compassesDict = compasses.ToDictionary(x => x.Type, x => x);
    }

    public void AddTarget(Transform target, ECompasTarget type)
    {
        compassesDict[type].AddTarget(target);
    }
    
    public void RemoveTarget(Transform target, ECompasTarget type)
    {
        compassesDict[type].RemoveTarget(target);
    }
}
