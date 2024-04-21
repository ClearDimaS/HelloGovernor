using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickPanel : UI_Panel
{
    [SerializeField] private Joystick joystick;

    public float Magnitude => joystick.Direction.magnitude;
    public bool IsMoving => joystick.Vertical != 0f && joystick.Horizontal != 0f;
    
    protected event Action<Vector2> inputEvent;

    public void SubscribeInput(Action<Vector2> handler)
    {
        inputEvent += handler;
    }
    
    private void Update()
    {
        if (joystick.Vertical != 0f || joystick.Horizontal != 0f)
        {
            inputEvent?.Invoke(joystick.Direction);
        }
    }
}
