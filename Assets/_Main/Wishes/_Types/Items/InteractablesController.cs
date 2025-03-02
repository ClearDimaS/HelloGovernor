using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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

            PlaceInteractables(false);
            RefreshRig();
            transform.position = animator.transform.position;
        }
        else
        {
            PlaceInteractables(true);
        }
    }

    private void RefreshRig()
    {
        if (interactor.HasAnyItem())
        {
            var localPlace = interactor.GetItemsRootLocalPlace();
            root.position = Vector3.Lerp(root.position, animator.transform.TransformPoint(localPlace), Time.deltaTime * 5f);
            target.position = root.position;
            if (handRig.weight < 1)
            {
                handRig.weight += Time.deltaTime * 3f;   
            }
        }
        else
        {
            if (handRig.weight > 0)
            {
                handRig.weight -= Time.deltaTime * 3f;   
            }
        }
    }

    private void PlaceInteractables(bool immediate)
    {
        var interactables = interactor.interactables;
        for (var i = 0; i < interactables.Count; i++)
        {
            var item = interactables[i];

            var place = interactor.GetPlace(i);
            if (item.transform.parent != place)
            {
                if (immediate)
                {
                    item.transform.DOKill();
                    item.transform.SetParent(place);
                    item.transform.localRotation = Quaternion.identity;
                    item.transform.localPosition = Vector3.zero;
                    continue;
                }
                item.transform.DOKill();
                item.transform.SetParent(place);
                var middle = (item.transform.position + place.position) / 2f;
                middle.y = place.position.y + 1f;
                item.transform.DOMove(middle, 0.3f).SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    item.transform.DOLocalMove(Vector3.zero, 0.15f).SetEase(Ease.InCubic).OnComplete(() =>
                    {
                        var startScale = item.transform.localScale;
                        item.transform.DOScale(startScale * 1.3f, 0.2f).SetEase(Ease.OutCubic).OnComplete(() =>
                        {
                            item.transform.DOScale(startScale, 0.2f).SetEase(Ease.InCubic);
                        });
                    });
                });
                item.transform.DOLocalRotate(Vector3.zero, 0.4f).SetEase(Ease.OutCubic);
            }
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
