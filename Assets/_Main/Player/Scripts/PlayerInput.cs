using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class PlayerInput : MonoBehaviour
{
    [Inject] private GameConfig gameConfig;

    private float lastPressTime = 0f;
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
        lastPressTime = Time.time;
        moveEvent?.Invoke((dir.y * gameConfig.moveForward + dir.x * gameConfig.moveRight));
    }

    public bool HasRecentPresser()
    {
        return Time.time - lastPressTime < gameConfig.moneySpendDelayAfterInput;
    }

    public async UniTaskVoid ForceMobile()
    {
        await UniTask.WaitUntil(() => joystickPanel != null);
        joystickPanel.ForceMobile();
    }
}
