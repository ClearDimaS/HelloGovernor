using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class AssistantAnimationSetter : CulledBehaviour
{
    [SerializeField] private Walker walker;
    [SerializeField, ShowIf(nameof(useAnimatorOverride))] private RuntimeAnimatorController animatorController;
    [SerializeField] private bool useAnimatorOverride = true;
    private int SpeedHash = Animator.StringToHash("Speed");
    private int IsWalkingHash = Animator.StringToHash("IsWalking");

    private Animator animator;

    protected override void OnAwake()
    {
        base.OnAwake();
        animator = GetComponentInChildren<Animator>();
        if (useAnimatorOverride)
        {
            animator.runtimeAnimatorController = animatorController;
        }
        animator.applyRootMotion = false;
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