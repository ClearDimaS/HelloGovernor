using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerInput : MonoBehaviour
{
    [Inject] private GameConfig gameConfig;
    
    public event Action<Vector3> moveEvent;

    private void Start()
    {
        UI_Manager.Instance.GetPanel<JoystickPanel>().SubscribeInput(OnInput);
    }

    private void OnInput(Vector2 dir)
    {
        dir = dir.normalized;
        moveEvent?.Invoke(dir.y * gameConfig.moveForward + dir.x * gameConfig.moveRight);
    }
}
