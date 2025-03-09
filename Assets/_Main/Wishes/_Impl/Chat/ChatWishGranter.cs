using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class ChatWishGranter : UIWishGranter
{
    [Inject] protected EnvironmentManager environment;
    [Inject] protected UI_Manager uiManager;
    
    [SerializeField] private float radius = 2f;
    [SerializeField] private float timeOut = 10f;
    protected float startTime = -1;

    protected bool IsTimeOut => processed.Count > 0 && Time.time - startTime < timeOut;

    protected override UI_Panel GetPanel()
    {
        return uiManager.GetPanel<ChatUI_Panel>();
    }

    protected override void OnActivate()
    {
        var chatMinigame = new ChatMinigame(config.reward * GetCitizensCount(), coolDownTimer.GetGameTimer());
        uiManager.GetPanel<ChatUI_Panel>().Show(chatMinigame);
        base.OnActivate();
    }

    protected override void OnStart()
    {
        base.OnStart();
        Reshuffle();
    }

    protected override void OnAddToProcessed(CitizenController citizen)
    {
        base.OnAddToProcessed(citizen);
        if (processed.Count == 1)
        {
            startTime = Time.time;
        }
        citizen.IsChatting = true;
    }

    public override bool CanAddOneMore()
    {
        return base.CanAddOneMore() && (!IsTimeOut);
    }

    protected override bool CanShowActivation_Internal()
    {
        return base.CanShowActivation_Internal() || IsTimeOut;
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
        if (!environment.IsReady)
        {
            UniTask.WaitUntil(() => environment.IsReady).ContinueWith(() =>
            {
                Reshuffle();
            });
            return;
        }
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