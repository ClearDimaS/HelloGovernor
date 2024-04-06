using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScreen : UI_Screen
{
    public override EScreenType Type => EScreenType.Lobby;

    [SerializeField] private float maxScale = 1.2f;
    [SerializeField] private float scaleDuration = 1f;
    [SerializeField] private Button startButton;

    protected override void OnAwake()
    {
        base.OnAwake();
        startButton.onClick.AddListener(GameManager.Instance.LaunchGame);
    }

    public override void OnShow()
    {
        base.OnShow();
        startButton.transform.DOKill();
        startButton.transform.localScale = Vector3.one;
        ScaleIn();
    }

    private void ScaleIn()
    {
        startButton.transform.DOScale(Vector3.one * maxScale, scaleDuration / 2f).OnComplete(() =>
        {
            ScaleOut();
        }).SetEase(Ease.Linear);
    }

    private void ScaleOut()
    {
        startButton.transform.DOScale(Vector3.one , scaleDuration / 2f).OnComplete(() =>
        {
            ScaleIn();
        }).SetEase(Ease.Linear);
    }
}
