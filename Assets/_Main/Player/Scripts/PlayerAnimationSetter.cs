using System;
using UnityEngine;
using Zenject;

public class PlayerAnimationSetter : MonoBehaviour
{
    private int SpeedHash = Animator.StringToHash("Speed");
    private int IsWalkingHash = Animator.StringToHash("IsWalking");
    
    [Inject] private PlayerSkinManager playerSkinManager;
    [Inject] private PlayerInput input;

    private void Update()
    {
        var animator = playerSkinManager.Animator;
        if (animator != null)
        {
            animator.SetFloat(SpeedHash, input.Magnitude);
            animator.SetBool(IsWalkingHash, input.IsMoving);   
        }
    }
}