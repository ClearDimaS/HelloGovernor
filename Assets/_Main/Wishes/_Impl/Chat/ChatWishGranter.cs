using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class ChatWishGranter : UIWishGranter<ChatUI_Panel>
{
    [Inject] protected EnvironmentManager environment;

    [SerializeField] private float radius = 2f;

    protected override void OnStart()
    {
        base.OnStart();
        Reshuffle();
    }

    protected override void OnAddToProcessed(CitizenController citizen)
    {
        base.OnAddToProcessed(citizen);
        citizen.IsChatting = true;
    }

    protected override void OnRemoveFromProcessed(CitizenController citizen)
    {
        base.OnRemoveFromProcessed(citizen);
        citizen.IsChatting = false;
    }

    protected override void OnResetActivation()
    {
        base.OnResetActivation();
        Reshuffle();
    }

    private void Reshuffle()
    {
        var middle = environment.GetRandomUnlockedPosition(radius);
        for (var i = 0; i < processPlaces.Length; i++)
        {
            var processPlace = processPlaces[i];
            processPlace.transform.position = middle + 
                                              Quaternion.AngleAxis(i/(float)processPlaces.Length * 360f, Vector3.up) * 
                                              Vector3.right * radius;
            processPlace.transform.rotation = Quaternion.LookRotation((middle - processPlace.Position).normalized, Vector3.up);
        }
        activationPlace.transform.position = middle;
    }
}