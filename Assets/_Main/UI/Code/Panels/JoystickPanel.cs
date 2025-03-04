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

    public float Magnitude => joystick.gameObject.activeSelf ? joystick.Direction.magnitude : 0f;
    public bool IsMoving => joystick.gameObject.activeSelf && 
                            (joystick.Vertical != 0f && joystick.Horizontal != 0f);
    
    protected event Action<Vector2> inputEvent;

    private void Start()
    {
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
        var activeJoystick = cameraManager.IsOnPlayer;
        if (activeJoystick != joystick.gameObject.activeSelf)
        {
            inputEvent?.Invoke(Vector2.zero);
            joystick.gameObject.SetActive(activeJoystick);
        }

        if (activeJoystick)
        {
            if (joystick.Vertical != 0f || joystick.Horizontal != 0f)
            {
                inputEvent?.Invoke(joystick.Direction);
            }
        }
    }
}
