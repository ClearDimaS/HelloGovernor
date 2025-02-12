using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class InteractablesController : CulledBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject rigPrefab;
    [SerializeField] private Rig handRig;
    [SerializeField] private Transform target;
    [SerializeField] private Transform root;

    private Interactor interactor;
    private bool reinit = true;

    protected override void OnAwake()
    {
        base.OnAwake();
        interactor = GetComponentInParent<Interactor>();
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (visible)
        {
            if (animator == null)
            {
                var getAnimatorFrom = transform.parent;
                animator = getAnimatorFrom.GetComponentInChildren<Animator>();
                reinit = true;
            }

            if (reinit && animator != null)
            {
                InitIK();
                reinit = false;
            }

            target.position = root.position;
            handRig.weight = interactor.HasAnyItem() ? 1f : 0f;
        }
    }

    private void InitIK()
    {
        var rigBuilder = animator.gameObject.GetComponent<RigBuilder>();
        if (rigBuilder == null)
        {
            rigBuilder = animator.gameObject.AddComponent<RigBuilder>();
        }

        if (handRig == null)
        {
            handRig = Instantiate(rigPrefab, rigBuilder.transform).GetComponent<Rig>();   
        }

        handRig.transform.SetParent(rigBuilder.transform);
        handRig.transform.localPosition = Vector3.zero;
        handRig.transform.localRotation = Quaternion.identity;
        handRig.transform.localScale = Vector3.one;
        
        rigBuilder.layers.Clear();
        rigBuilder.layers.Add(new (handRig, true));      
    
                
        var twoBoneIK = handRig.GetComponentInChildren<TwoBoneIKConstraint>();
        target = twoBoneIK.data.target;
        twoBoneIK.Reset();
        twoBoneIK.data.root = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        twoBoneIK.data.mid = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        twoBoneIK.data.tip = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        twoBoneIK.data.target = target;
                
        rigBuilder.enabled = true;
        rigBuilder.Build();
        animator.Rebind();
    }
}
