using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantScale : CulledBehaviour
{
    [SerializeField] private AnimationCurve scaleCurve = new AnimationCurve(new Keyframe[] {new Keyframe(0, 1), new Keyframe(0.5f, 1.2f), new Keyframe(1f, 1f)});
    private float period;
    private float startScale;
    private float t;

    protected override void OnAwake()
    {
        base.OnAwake();
        period = scaleCurve.keys[scaleCurve.keys.Length - 1].time;
        startScale = transform.localScale.x;
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (visible)
        {
            t += Time.deltaTime;
            if (t > period)
            {
                t -= period;
            }
            transform.localScale = scaleCurve.Evaluate(t) * Vector3.one * startScale;
        }
    }
}
