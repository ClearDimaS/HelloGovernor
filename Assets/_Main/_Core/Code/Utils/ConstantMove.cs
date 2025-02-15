using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantMove : CulledBehaviour
{
    [SerializeField] private AnimationCurve moveCurve = new AnimationCurve(new Keyframe[] {new Keyframe(0, 1), new Keyframe(0.5f, 1.2f), new Keyframe(1f, 1f)});
    [SerializeField] private Vector3 axis;
    private float period;
    private float t;

    protected override void OnAwake()
    {
        base.OnAwake();
        period = moveCurve.keys[moveCurve.keys.Length - 1].time;
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
            transform.localPosition = moveCurve.Evaluate(t) * axis;
        }
    }
}
