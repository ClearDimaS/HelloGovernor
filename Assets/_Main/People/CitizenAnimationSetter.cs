using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CitizenAnimationSetter : CitizenBehaviour
{
    [SerializeField] private CitizenController citizenController;
    [SerializeField] private Walker walker;
    [SerializeField] private RuntimeAnimatorController animatorController;
    
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    private static readonly int IsChattingHash = Animator.StringToHash("IsChatting");
    private static readonly int IsSitting = Animator.StringToHash("IsSitting");

    private Animator animator;
    
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        animator.runtimeAnimatorController = animatorController;
        animator.SetFloat("SitRandomSpeedMult", Random.Range(0.5f, 2f));
    }

    public override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (animator != null)
        {
            animator.SetBool(IsChattingHash, citizenController.IsChatting);
            animator.SetFloat(SpeedHash, walker.Speed);
            animator.SetBool(IsWalkingHash, walker.IsMoving);   
            animator.SetBool(IsSitting, citizenController.WishesController.IsSitting);
        }
    }
}
