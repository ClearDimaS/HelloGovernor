using System.Collections.Generic;
using UnityEngine;

public class AnimatedWishPostProcessor : WishPostProcessor
{
    [SerializeField] protected string AnimName;
    [SerializeField] protected float stayTime = 3f;
    [SerializeField] private WishPostProcessorAnimator postProcessorAnimator;
    [SerializeField] protected List<Transform> processPlaces;
    [SerializeField] private List<CitizenController> citizens = new ();
    
    private float processingTime;
    
    public override void Add(CitizenController processed)
    {
        var teleportTo = processPlaces[citizens.Count];

        processed.Animator.transform.position = teleportTo.position;
        processed.Animator.transform.rotation = teleportTo.rotation;
        if (!string.IsNullOrEmpty(AnimName))
        {
            processed.PlayAnimation(AnimName);
        }
        citizens.Add(processed);
        processed.WishesController.DisableAllDesires();
    }
    
    public override bool IsProcessing(CitizenController citizen)
    {
        return citizens.Contains(citizen);
    }
    
    public override void OnUpdate()
    {
        for (int i = 0; i < citizens.Count; i++)
        {
            var teleportTo = processPlaces[i];
            var processed = citizens[i];
            processed.Animator.transform.position = teleportTo.position;
            processed.Animator.transform.rotation = teleportTo.rotation;
        }
        
        if (citizens.Count == processPlaces.Count)
        {
            postProcessorAnimator.Move();
            processingTime += Time.deltaTime;
            if (processingTime >= stayTime)
            {
                processingTime = 0f;
                postProcessorAnimator.StopMove();
                var count = processPlaces.Count;
                for (int i = 0; i < count; i++)
                {
                    Remove(0);
                }
            }
        }
    }

    public override bool HasMorePlace()
    {
        return citizens.Count < processPlaces.Count;
    }
    
    private void Remove(int index)
    {
        var citizen = citizens[index];
        if (!string.IsNullOrEmpty(AnimName))
        {
            citizen.ResetAnimation();
        }
        citizen.Animator.transform.localPosition = Vector3.zero;
        citizen.Animator.transform.localRotation = Quaternion.identity;
        citizens.RemoveAt(index);
        citizen.WishesController.EnableAllDesires();
    }
}