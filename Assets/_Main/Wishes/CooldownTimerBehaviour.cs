using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public interface ICooldownable
{
    public float CoolDown { get; }
    public float CoolDownTimeLeft { get; }
    public bool IsCooldown { get; }
}

public class CooldownTimerBehaviour : CulledBehaviour
{
    [SerializeField] private TimerTextMesh textSS;
    [SerializeField] private TimerTextMesh textMM;
    [SerializeField] private GameObject content;
    
    private ICooldownable wishGranter;

    protected int lastTimeLeft = -1;

    private void Start()
    {
        wishGranter = GetComponentInParent<ICooldownable>();
        if (wishGranter.CoolDown <= 0f)
        {
            Destroy(gameObject);
        }
    }

    protected override void OnUpdate(bool visible)
    {
        if (visible)
        {
            var showTimer = wishGranter.IsCooldown;
            if (showTimer != content.activeSelf)
            {
                content.SetActive(showTimer);
            }
            if (showTimer)
            {
                var timeLeft = Mathf.RoundToInt(wishGranter.CoolDownTimeLeft);
                if (lastTimeLeft != timeLeft)
                {
                    lastTimeLeft = timeLeft;
                    var ss = timeLeft % 60;
                    var mm = timeLeft / 60;
                    textSS.SetValue(ss);
                    textMM.SetValue(mm);
                }
            }
        }
    }
}