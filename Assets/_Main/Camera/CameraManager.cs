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
        private Action startCallback;
        public bool HasTimer { get; private set; }

        public CameraTarget(Transform target, float timer, Action startCallback)
        {
            this.target = target;
            this.timer = timer;
            HasTimer = timer > 0f;
            this.startCallback = startCallback;
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
    [field: SerializeField] public Camera UI_Camera { get; private set; }
    
    public Camera ActiveCamera => _currentCamera;
    public Camera OriginalCamera => camera;

    private Camera _currentCamera;

    private Transform targetPlaceHolder;
    
    private bool isTransition;
    private Transform defaultTarget;
    private CameraTarget currentTarget;
    private Queue<CameraTarget> targetsQueue = new ();

    private void Awake()
    {
        _currentCamera = camera;
        var targetPlaceHolderGO = new GameObject("Target Placeholder");
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
        if (currentTarget.HasTimer && !isTransition)
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

    public void SetTarget(Transform target, float timer, float delay = -1f, Action startCallback = null)
    {
        if ((currentTarget.HasTimer && currentTarget.timer > 0f))
        {
            if (delay > 0f)
            {
                currentTarget.OverrideTimer(Mathf.Max(currentTarget.timer, delay));
            }
            targetsQueue.Enqueue(new CameraTarget(target, timer, startCallback));
        }
        else
        {
            var newTarget = new CameraTarget(target, timer, startCallback);
            if (delay > 0f)
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
        ApplyTarget(new CameraTarget(defaultTarget, -1f, null), instant);
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
            DOTween.To(() => t, x => t = x, 1f, moveTime).OnUpdate(() =>
            {
                targetPlaceHolder.position = Vector3.Lerp(startPos, currentTarget.target.position, t);
            }).OnComplete(() =>
            {
                isTransition = false;
            }).SetEase(gameConfig.cameraTransitionEase);
        }
    }
}
