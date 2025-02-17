using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class ScaleAnimator : MonoBehaviour
{
    [SerializeField] private float pause = 0.05f;
    [SerializeField] private float heightAnimationOffset = 0.4f;
    [SerializeField] private float animationDuration = 0.3f;

    [SerializeField] private MeshRenderer[] mrs;
    [SerializeField] private Vector3[] localPositions;
    [SerializeField] private Vector3[] localScales;
    
    private int state = -1;
    private bool isInit = false;
    
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (isInit)
        {
            return;
        }

        isInit = true;
        mrs = mrs.Where(x => x != null).ToArray();
        localPositions = mrs.Select(x => x.transform.localPosition).ToArray();
        localScales = mrs.Select(x => x.transform.localScale).ToArray();
    }

    [Button]
    private void CollectMRs()
    {
        mrs = GetComponentsInChildren<MeshRenderer>();
        localPositions = mrs.Select(x => x.transform.localPosition).ToArray();
        localScales = mrs.Select(x => x.transform.localScale).ToArray();
    }
    
    [Button]
    public void Show(bool instant)
    {
        if (state == 1)
        {
            return;
        }

        Init();
        state = 1;
        if (instant)
        {
            for (var i = 0; i < mrs.Length; i++)
            {
                var mr = mrs[i];
                mr.transform.localPosition = localPositions[i];
                mr.transform.localScale = localScales[i];
            }
        }
        else
        {
            for (var i = 0; i < mrs.Length; i++)
            {
                var mr = mrs[i];
                mr.transform.localPosition = localPositions[i] + Vector3.up * heightAnimationOffset;
            }
            ShowCoroutine().ToUniTask();
        }
    }

    [Button]
    public void Hide(bool instant)
    {
        if (state == 0)
        {
            return;
        }
        
        Init();
        state = 0;
        if (instant)
        {
            for (var i = 0; i < mrs.Length; i++)
            {
                var mr = mrs[i];
                mr.transform.localPosition = localPositions[i];
                mr.transform.localScale = Vector3.zero;
            }
        }
        else
        {
            HideCoroutine().ToUniTask();
        }
    }

    private IEnumerator ShowCoroutine()
    {
        for (var i = 0; i < mrs.Length; i++)
        {
            var mr = mrs[i];
            mr.gameObject.SetActive(true);
            mr.transform.DOKill();
            mr.transform.DOLocalMove(localPositions[i], animationDuration);
            mr.transform.DOScale(localScales[i], animationDuration);
            if (pause > 0f)
            {
                yield return new WaitForSeconds(pause);   
            }
        }
    }
    
    private IEnumerator HideCoroutine()
    {
        for (var i = 0; i < mrs.Length; i++)
        {
            var mr = mrs[i];
            mr.transform.DOLocalMove(localPositions[i] + Vector3.up * heightAnimationOffset, animationDuration);
            mr.transform.DOScale(Vector3.zero, animationDuration).OnComplete(() => mr.gameObject.SetActive(false));
            if (pause > 0f)
            {
                yield return new WaitForSeconds(pause);   
            }
        }
    }
}
