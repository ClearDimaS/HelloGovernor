using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public abstract class CitizenBehaviour : MonoBehaviour
{
    public abstract void OnReset();
    public virtual void OnUpdate(bool visible)
    {

    }
}

public class CitizenController : CulledBehaviour
{
    [Inject] private PlayerInput playerInput;
    [Inject] private PlayerController player;
    [Inject] private EnvironmentManager environment;
    
    [SerializeField] private Walker walker;
    [SerializeField] private WishesController wishesController;
    [SerializeField] private Interactor interactor;

    private CitizenBehaviour[] behaviours;
    private HouseBuilding house;

    public Animator Animator { get; private set; }
    public bool IsChatting { get; set; }
    public Walker Walker => walker;
    public WishesController WishesController => wishesController;
    protected bool isPushed;
    protected float lastPushTime;

    protected override void OnAwake()
    {
        base.OnAwake();
        Animator = GetComponentInChildren<Animator>();
        behaviours = GetComponentsInChildren<CitizenBehaviour>();
    }

    protected override void OnOnEnable()
    {
        base.OnOnEnable();
        foreach (var behaviour in behaviours)
        {
            behaviour.OnReset();
        }
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        foreach (var bhvr in behaviours)
        {
            bhvr.OnUpdate(visible);
        }

        if (visible)
        {
            if (!isPushed && walker.IsMoving && playerInput.IsMoving)
            {
                var diff = transform.position - player.transform.position;
                diff.y = 0f;
                var isMovingToMe = Vector3.Dot(diff.normalized, player.transform.forward) > dotTreshold;
                if (diff.magnitude < pushedRadius)
                {
                    lastPushTime = Time.time;
                    GetPushed(-diff.normalized);
                }   
            }

            if (isPushed && Time.time - lastPushTime > pushTimeout)
            {
                isPushed = false;
            }
        }
    }

    public void PlaceRandom()
    {
        if (!environment.IsReady)
        {
            UniTask.WaitUntil(() => environment.IsReady).ContinueWith(() =>
            {
                var pos = environment.GetRandomUnlockedPosition(0f);
                walker.Place(pos);
            });
            return;
        }
        var pos = environment.GetRandomUnlockedPosition(0f);
        walker.Place(pos);
    }

    public void Place(Vector3 citizenPlacePosition)
    {
        walker.Place(citizenPlacePosition);
    }

    public void AddItem(CitizenItem item)
    {
        if (interactor.interactables.Count > 0)
        {
            interactor.ClearItems();
        }
        interactor.AddItem(item);
        item.PoolPleaseAtTimeout(() =>
        {
            interactor.RemoveItem(item);
        });
    }

    private float pushTimeout = 35f;
    protected float dotTreshold = 0.2f;
    protected float pushedRadius = 1f;
    protected Vector2 pushedTimeMinMax = new Vector2(3, 6);
    protected float recoverTime = 2.3f;
    protected void GetPushed(Vector3 dir)
    {
        isPushed = true;
        var isSide = Mathf.Abs(dir.x) > Mathf.Abs(dir.z);
        if (isSide)
        {
            Animator.CrossFade("SidePushed", 0.1f);
        }
        else
        {
            var isFront = dir.z > 0;
            if (isFront)
            {
                Animator.CrossFade("FrontPushed", 0.1f);
            }
            else
            {
                Animator.CrossFade("BackPushed", 0.1f);
            }
        }

        walker.SetSpeedMult(0f);
        var knockdownTime = Random.Range(pushedTimeMinMax.x, pushedTimeMinMax.y);
        UniTask.Delay(TimeSpan.FromSeconds(knockdownTime)).ContinueWith(Recover);
    }

    private void Recover()
    {
        if (IsVisible)
        {
            Animator.CrossFade("GetUp", 0.1f);
        }
        else
        {
            Animator.Play("Idle");
        }
        UniTask.Delay(TimeSpan.FromSeconds(recoverTime)).ContinueWith(() =>
        {
            walker.SetSpeedMult(1f);
        });
    }

    public void PlayAnimation(string animName)
    {
        if (IsVisible)
        {
            Animator.CrossFade(animName, 0.01f);
        }
        else
        {
            Animator.Play(animName);
        }
    }

    public void ResetAnimation()
    {
        if (IsVisible)
        {
            Animator.CrossFade("Idle", 0.01f);
        }
        else
        {
            Animator.Play("Idle");
        }
    }
}
