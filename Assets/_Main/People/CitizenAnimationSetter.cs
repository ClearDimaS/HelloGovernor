using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenAnimationSetter : MonoBehaviour
{
    [SerializeField] private Walker walker;
    [SerializeField] private RuntimeAnimatorController animatorController;
    
    private int SpeedHash = Animator.StringToHash("Speed");
    private int IsWalkingHash = Animator.StringToHash("IsWalking");

    private Animator animator;
    
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        animator.runtimeAnimatorController = animatorController;
    }

    private void Update()
    {
        if (animator != null)
        {
            animator.SetFloat(SpeedHash, walker.Speed);
            animator.SetBool(IsWalkingHash, walker.IsMoving);   
        }
    }
}
