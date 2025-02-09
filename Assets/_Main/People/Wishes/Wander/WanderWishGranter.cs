using System;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class WanderWishGranter : WishGranter
{
    [Inject] private EnvironmentManager environment;

    protected override void UpdateProcessed(CitizenController citizen)
    {
        base.UpdateProcessed(citizen);
        if (!citizen.Walker.IsMoving)
        {
            var pos = GetRandomPos();
            citizen.Walker.MoveToTarget(pos, null);
        }
    }

    protected override bool CanAddProgress(CitizenController citizen)
    {
        return true;
    }

    protected override Vector3 GetQueuePlaceFor(CitizenController citizen)
    {
        return citizen.transform.position;
    }

    protected override Vector3 GetProcessPlaceFor(CitizenController citizen)
    {
        return citizen.transform.position;
    }
    
    protected override Quaternion GetProcessRotFor(CitizenController citizen)
    {
        return citizen.transform.rotation;
    }
    
    protected override Vector3 GetExitPlaceFor(CitizenController citizen)
    {
        return citizen.transform.position;
    }
    
    private Vector3 GetRandomPos()
    {
        var pos = environment.mapCenter + new Vector3(
            Random.Range(-environment.mapSize.x / 2f, environment.mapSize.x / 2f),
            0,
            Random.Range(-environment.mapSize.z / 2f, environment.mapSize.z / 2f));
        return pos;
    }
    
    public override bool CanAddOneMore()
    {
        return true;
    }
}