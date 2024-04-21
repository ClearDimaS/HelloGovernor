using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerInput : MonoBehaviour
{
    [Inject] private GameConfig gameConfig;
    
    public event Action<Vector3> moveEvent;

    public float Magnitude => joystickPanel.Magnitude;
    public bool IsMoving => joystickPanel.IsMoving;

    private JoystickPanel joystickPanel;
    
    private void Start()
    {
        joystickPanel = UI_Manager.Instance.GetPanel<JoystickPanel>();
        joystickPanel.SubscribeInput(OnInput);
    }

    private void OnInput(Vector2 dir)
    {
        moveEvent?.Invoke((dir.y * gameConfig.moveForward + dir.x * gameConfig.moveRight));
    }
}
