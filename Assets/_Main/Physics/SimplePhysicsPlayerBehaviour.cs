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

public class SimpleWishPlacePhysicsBehaviour : SimplePhysicsBehaviour<WishPlace>
{
    protected override List<WishPlace> GetComponentsForWork()
    {
        return FindObjectsOfType<WishPlace>(true).ToList();
    }
}

public class SimpleRepairerPhysicsBehaviour : SimplePhysicsBehaviourInterface<IRepairer>
{
    protected override List<IRepairer> GetComponentsForWork()
    {
        return FindObjectsOfType<RepairAssistant>(true).Select(x => x as IRepairer).
            Concat(new []{(IRepairer)PlayerController.Instance}).
            ToList();
    }
}

public class SimpleThiefBusterPhysicsBehaviour : SimplePhysicsBehaviourInterface<IThiefBuster>
{
    protected override List<IThiefBuster> GetComponentsForWork()
    {
        return FindObjectsOfType<CopAssistant>(true).Select(x => x as IThiefBuster).
            Concat(new []{(IThiefBuster)PlayerController.Instance}).
            ToList();
    }
}

public class SimpleItemsTakerPhysicsBehaviour : SimplePhysicsBehaviourInterface<IItemTaker>
{
    protected override List<IItemTaker> GetComponentsForWork()
    {
        return FindObjectsOfType<WaiterAssistant>(true).Select(x => x as IItemTaker).
            Concat(new []{(IItemTaker)PlayerController.Instance.GetComponentInChildren<MultipleItemsTaker>(true)}).
            ToList();
    }
}

