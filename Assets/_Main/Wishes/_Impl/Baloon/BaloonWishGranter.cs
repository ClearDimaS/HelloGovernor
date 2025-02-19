using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaloonWishGranter : OperatableWithItems
{
    protected float allGatherTime = -1f;

    protected override void OnUpdate()
    {
        base.OnUpdate();
        var allHere = processPlaces.Length == processed.Count;
        if (allGatherTime < 0f && allHere)
        {
            allGatherTime = Time.time;
        }

        if (Time.time - allGatherTime > config.grantDuration)
        {
            foreach (var processPlace in processPlaces)
            {
                processPlace.GetOwner().WishesController.AddProgress(this, 1f);
            }
            allGatherTime = -1f;
        }
    }

    protected override bool CanAddProgress(CitizenController citizen)
    {
        return base.CanAddProgress(citizen) && false;
    }
}
