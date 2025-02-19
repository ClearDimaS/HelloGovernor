using System;
using DG.Tweening;
using UnityEngine;

public class AddedMoneyElement : MonoBehaviour
{
    [SerializeField] private TextMesh textChange;
    [SerializeField] private Color startColor;
    [SerializeField] private string format = "+{0} $";
    private Color endColor;
    
    private void Awake()
    {
        startColor = textChange.color;
        gameObject.SetActive(false);
        endColor = startColor;
        endColor.a = 0f;
    }

    public void Show(int showChange, Transform startPlace, Transform endPlace, float moveTime, float fadeDelay, CameraManager cameraManager, Action onComplete)
    {
        gameObject.SetActive(true);
        textChange.text = string.Format(format,  Price.ToMoneyString(showChange));
        textChange.color = startColor;
        transform.position = startPlace.position;
        var t = 0f;
        transform.forward = cameraManager.ActiveCamera.transform.forward;
        DOTween.To(() => t, x => t = x, 1f, moveTime).OnUpdate(() =>
        {
            transform.position = Vector3.Lerp(startPlace.position, endPlace.position, t);
            transform.forward = cameraManager.ActiveCamera.transform.forward;
        }).OnComplete(() =>
        {
            onComplete?.Invoke();
            gameObject.SetActive(false);
        });
        var t0 = 0f;
        DOTween.To(() => t0, x => t0 = x, 1f, moveTime - fadeDelay).OnUpdate(() =>
        {
            textChange.color = Color.Lerp(startColor, endColor, t0);
        }).SetDelay(fadeDelay);
    }
}