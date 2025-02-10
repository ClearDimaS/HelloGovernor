using System;
using UnityEngine;

public class AssistantAnimationSetter : CulledBehaviour
{
    [SerializeField] private Walker walker;
    [SerializeField] private RuntimeAnimatorController animatorController;
    
    private int SpeedHash = Animator.StringToHash("Speed");
    private int IsWalkingHash = Animator.StringToHash("IsWalking");

    private Animator animator;

    protected override void OnAwake()
    {
        base.OnAwake();
        animator = GetComponentInChildren<Animator>();
        animator.runtimeAnimatorController = animatorController;
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (visible)
        {
            if (animator != null)
            {
                animator.SetFloat(SpeedHash, walker.Speed);
                animator.SetBool(IsWalkingHash, walker.IsMoving);   
            }
        }
    }
}