using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BankRewardChangeElement : MonoBehaviour
{
    [SerializeField] private CanvasGroup changeRoot;
    [SerializeField] private TMP_Text changeText;
    
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private float fadeDelay = 0.8f;

    private void Awake()
    {
        changeRoot.alpha = 0f;
    }

    public void Init(RectTransform changeStartPlace, RectTransform changeEndPlace, int change, Action onDone)
    {
        changeText.text = change.ToString();
        
        changeRoot.transform.position = changeStartPlace.position;
        changeRoot.transform.rotation = changeStartPlace.rotation;
        var t = 0f;

        DOTween.To(() => t, x => t = x, 1f, moveDuration).OnUpdate(() =>
        {
            var pos = Vector3.Lerp(changeStartPlace.position, changeEndPlace.position, t);
            var rot = Quaternion.Lerp(changeStartPlace.rotation, changeEndPlace.rotation, t);
            changeRoot.transform.position = pos;
            changeRoot.transform.rotation = rot;
        });
        
        changeRoot.alpha = 1f;
        changeRoot.DOFade(0f, moveDuration - fadeDelay).SetDelay(fadeDelay).OnComplete(() =>
        {
            var fire = onDone;
            fire?.Invoke();
            onDone = null;
        });
    }
}