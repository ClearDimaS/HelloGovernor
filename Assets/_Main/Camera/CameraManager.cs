using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MoreMountains.Tools;
using UnityEngine;
using Zenject;

public class CameraManager : MonoBehaviour, ICameraManager
{
    public class CameraTarget
    {
        public Transform target;
        public float timer;
        public float distanceMult;
        private Action startCallback;
        public bool HasTimer { get; private set; }

        public CameraTarget(Transform target, float timer, Action startCallback, float distanceMult)
        {
            this.target = target;
            this.timer = timer;
            HasTimer = timer > 0f;
            this.startCallback = startCallback;
            this.distanceMult = distanceMult;
        }

        public void OverrideTimer(float timer)
        {
            this.timer = timer;
            HasTimer = timer > 0f;
        }

        public void FireStart()
        {
            startCallback?.Invoke();
            startCallback = null;
        }
    }

    [Inject] private GameConfig gameConfig;
    
    [SerializeField] private MMFollowTarget targetFollower;
    [SerializeField] private Camera camera;
    [field: SerializeField] public Camera UI_Camera => camera;
    
    public Camera ActiveCamera => _currentCamera;
    public Camera OriginalCamera => camera;
    public bool IsOnPlayer { get; private set; }

    private Camera _currentCamera;

    private Transform targetPlaceHolder;
    
    private bool isTransition;
    private Vector3 originalOffset;
    private Transform defaultTarget;
    private CameraTarget currentTarget;
    private Queue<CameraTarget> targetsQueue = new ();
    public int BlockersCount { get; set; }
    private bool IsBlocked => BlockersCount > 0;

    private void Awake()
    {
        _currentCamera = camera;
        var targetPlaceHolderGO = new GameObject("Target Placeholder");
        originalOffset = targetFollower.Offset;
        targetPlaceHolder = targetPlaceHolderGO.transform;
        targetPlaceHolder.SetParent(transform);
        
        defaultTarget = targetFollower.Target;
        targetPlaceHolder.transform.position = defaultTarget.position;
        targetPlaceHolder.transform.rotation = defaultTarget.rotation;
        
        targetFollower.Target = targetPlaceHolder;
        
        SetDefaultTarget(true);
    }

    private void Update()
    {
        if (currentTarget.HasTimer && !isTransition && !IsBlocked)
        {
            currentTarget.timer -= Time.deltaTime;
            if (currentTarget.timer < 0f)
            {
                if (targetsQueue.Count > 0)
                {
                    var newTarget = targetsQueue.Dequeue();
                    ApplyTarget(newTarget, false);
                }
                else
                {
                    SetDefaultTarget(false);
                }
            }
        }

        IsOnPlayer = targetsQueue.Count == 0 && !isTransition && currentTarget.target == defaultTarget;

        if (!isTransition)
        {
            currentTarget.FireStart();
            targetPlaceHolder.position = currentTarget.target.position;
        }
    }

    private void LateUpdate()
    {
        if (!isTransition)
        {
            //targetPlaceHolder.position = currentTarget.target.position;
        }
    }

    public void SetActiveCamera(Camera cam)
    {
        _currentCamera = cam;
    }

    public void SetTarget(Transform target, float timer, float delay = -1f, Action startCallback = null, float distanceMult = 1f)
    {
        if ((currentTarget.HasTimer && currentTarget.timer > 0f))
        {
            if (delay > 0f)
            {
                currentTarget.OverrideTimer(Mathf.Max(currentTarget.timer, delay));
            }
            targetsQueue.Enqueue(new CameraTarget(target, timer, startCallback, distanceMult));
        }
        else
        {
            var newTarget = new CameraTarget(target, timer, startCallback, distanceMult);
            if (delay > 0f || IsBlocked)
            {
                currentTarget.OverrideTimer(Mathf.Max(currentTarget.timer, delay));
                targetsQueue.Enqueue(newTarget);
            }
            else
            {
                ApplyTarget(newTarget, false);
            }
        }
    }

    private void SetDefaultTarget(bool instant)
    {
        ApplyTarget(new CameraTarget(defaultTarget, -1f, null, 1f), instant);
    }
    
    private void ApplyTarget(CameraTarget targetData, bool instant)
    {
        currentTarget = targetData;
        if (instant)
        {
            isTransition = false;
        }
        else
        {
            isTransition = true;
            var dist = (targetPlaceHolder.position - targetData.target.position).magnitude;
            var moveTime = Mathf.Min(gameConfig.cameraTransitionMaxTime, dist / gameConfig.cameraTransitionSpeed);
            var t = 0f;
            var startPos = targetPlaceHolder.position;

            var startOffset = targetFollower.Offset;
            DOTween.To(() => t, x => t = x, 1f, moveTime).OnUpdate(() =>
            {
                targetFollower.Offset = Vector3.Lerp(startOffset, originalOffset * targetData.distanceMult, t);
                targetPlaceHolder.position = Vector3.Lerp(startPos, currentTarget.target.position, t);
            }).OnComplete(() =>
            {
                targetFollower.Offset = originalOffset * targetData.distanceMult;
                isTransition = false;
            }).SetEase(gameConfig.cameraTransitionEase);
        }
    }
}
