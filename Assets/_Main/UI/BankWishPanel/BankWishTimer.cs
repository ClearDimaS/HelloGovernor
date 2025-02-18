using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BankWishTimer : MonoBehaviour
{
    [SerializeField] private float scaleTime = 0.1f;
    [SerializeField] private float scale = 1.15f;

    [SerializeField] private TMP_Text textSS;
    [SerializeField] private TMP_Text textMM;

    private Dictionary<int, string> timeStrings = new();

    private int lastTime = -1;
    protected int lastSS = -1;
    private int lastMM = -1;

    private void OnEnable()
    {
        lastSS = -1;
        lastMM = -1;
    }

    public void SetTimeLeft(int time)
    {
        if (time == lastTime)
        {
            return;
        }

        lastTime = time;
        var ss = lastTime % 60;
        var mm = lastTime / 60;

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