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
    [SerializeField] private float scaleTime = 0.1f;
    [SerializeField] private float scale = 1.15f;
    
    [SerializeField] private TextMesh textSS;
    [SerializeField] private TextMesh textMM;

    private ICooldownable wishGranter;
    private Dictionary<int, string> timeStrings = new ();

    protected int lastTimeLeft = -1;

    protected int lastSS = -1;
    private int lastMM = -1;
    
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
            if (showTimer)
            {
                var timeLeft = Mathf.RoundToInt(wishGranter.CoolDownTimeLeft);
                if (lastTimeLeft != timeLeft)
                {
                    lastTimeLeft = timeLeft;
                    var ss = timeLeft % 60;
                    var mm = timeLeft / 60;

                    var secondsString = GetTimeString(ss);
                    var minutesString = GetTimeString(mm);
                    if (lastSS != ss)
                    {
                        lastSS = ss;
                        textSS.transform.DOScale(Vector3.one * scale, scaleTime).OnComplete(() =>
                        {
                            textSS.text = secondsString;
                            textSS.transform.DOScale(Vector3.one, scaleTime);
                        });
                    }

                    if (lastMM != mm)
                    {
                        lastMM = mm;
                        textMM.transform.DOScale(Vector3.one * scale, scaleTime).OnComplete(() =>
                        {
                            textMM.text = minutesString;
                            textMM.transform.DOScale(Vector3.one, scaleTime);
                        });
                    }
                }
            }
        }
    }

    private string GetTimeString(int v)
    {
        if (!timeStrings.ContainsKey(v))
        {
            var str = v.ToString();
            if (str.Length == 1)
            {
                str = "0" + str;
            }
            timeStrings[v] = str;
        }
        return timeStrings[v];
    }
}