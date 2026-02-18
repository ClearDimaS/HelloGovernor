using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class JoystickPanel : UI_Panel
{
    [Inject] private CameraManager cameraManager;
    [SerializeField] private Joystick joystick;

    public float Magnitude => lastMagn.magnitude;
    public bool IsMoving => (lastDIr.y != 0f || lastDIr.x != 0f);

    private bool forceMobile;
    protected Vector2 lastDIr;
    protected Vector2 lastMagn;
    protected event Action<Vector2> inputEvent;

    private void Start()
    {
#if UNITY_IOS || UNITY_ANDROID
#else
        joystick.gameObject.SetActiveOnce(false);
        #endif

        joystick.OnPointerDown(new PointerEventData(EventSystem.current));

        UniTask.DelayFrame(1).ContinueWith(() =>
        {
            joystick.OnPointerUp(new PointerEventData(EventSystem.current));
        });
    }

    public void SubscribeInput(Action<Vector2> handler)
    {
        inputEvent += handler;
    }
    
    private void Update()
    {
#if UNITY_WEBGL
        if (forceMobile)
        {
            UpdateMobile();
        }
        else
        {
            UpdateDesktop();
        }
        #else
        UpdateMobile();
        #endif
    }

    void UpdateMobile()
    {
        var activeJoystick = cameraManager.IsOnPlayer;
        if (activeJoystick != joystick.gameObject.activeSelf)
        {
            inputEvent?.Invoke(Vector2.zero);
            joystick.OnPointerUp(new PointerEventData(EventSystem.current));
            joystick.gameObject.SetActive(activeJoystick);
        }
        if (activeJoystick)
        {
            if (joystick.Vertical != 0f || joystick.Horizontal != 0f)
            {
                inputEvent?.Invoke(joystick.Direction);
                lastMagn = joystick.gameObject.activeSelf ? joystick.Direction : Vector2.zero;
            }
            else
            {
                lastMagn = Vector2.zero;
            }
            lastDIr = joystick.Direction;

        }          
        else
        {
            lastMagn = Vector2.zero;
            lastDIr = Vector2.zero;
        }
    }

    void UpdateDesktop()
    {
        var activeJoystick = cameraManager.IsOnPlayer;
        joystick.gameObject.SetActiveOnce(false);
        if (activeJoystick)
        {
            
            var dir = Vector2.zero;
            if (Input.GetKey(KeyCode.W))
            {
                dir += Vector2.up;
            }
            if (Input.GetKey(KeyCode.A))
            {
                dir += Vector2.left;
            }
            if (Input.GetKey(KeyCode.S))
            {
                dir += Vector2.down;
            }
            if (Input.GetKey(KeyCode.D))
            {
                dir += Vector2.right;  
            }

            if (dir.sqrMagnitude > 0.1f)
            {
                inputEvent?.Invoke(dir.normalized);
            }

            lastDIr = dir;
            lastMagn = dir;
        }          
        else
        {
            lastMagn = Vector2.zero;
            lastDIr = Vector2.zero;
        }
    }

    public void ForceMobile()
    {
        forceMobile = true;
    }
}
