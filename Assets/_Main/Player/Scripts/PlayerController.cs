using System;
using UnityEngine;
using Zenject;

public class PlayerController : MonoBehaviour, IMoneySpender, IRepairer
{
    [Inject] private GameConfig gameConfig;
    [Inject] private PlayerInput playerInput;

    [SerializeField] private Rigidbody rb;

    private int frameRequiredDelta;

    private Vector3 delta;
    
    private void Awake()
    {
        playerInput.moveEvent += RequireMove;
    }

    private void RequireMove(Vector3 delta)
    {
        this.delta = delta;
        frameRequiredDelta = Time.frameCount;
    }

    private void FixedUpdate()
    {
        if (Time.frameCount > frameRequiredDelta + 1)
        {
            return;
        }

        var dir = delta.normalized;
        rb.rotation = Quaternion.Lerp(rb.rotation, Quaternion.LookRotation(dir, Vector3.up), 1f);
        rb.MovePosition(rb.position + delta * Time.fixedDeltaTime * gameConfig.playerSpeed);
    }
}