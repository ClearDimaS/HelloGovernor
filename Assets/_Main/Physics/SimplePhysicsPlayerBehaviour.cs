using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class SimplePlayerPhysicsBehaviour : SimplePhysicsBehaviour<PlayerController>
{
    protected override List<PlayerController> GetComponentsForWork()
    {
        return new List<PlayerController>() { PlayerController.Instance };
    }
}

public class SimpleRepairerPhysicsBehaviour : SimplePhysicsBehaviourInterface<IRepairer>
{
    public override Transform Center => myCenter;
    protected virtual Transform myCenter => transform;

    protected override List<IRepairer> GetComponentsForWork()
    {
        return FindObjectsOfType<RepairAssistant>(true).Select(x => x as IRepairer).
            Concat(new []{(IRepairer)PlayerController.Instance}).
            ToList();
    }
}


