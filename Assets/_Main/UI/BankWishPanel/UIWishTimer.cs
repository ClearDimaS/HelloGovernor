using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIWishTimer : MonoBehaviour
{
    [SerializeField] private TimerTextTMP textSS;
    [SerializeField] private TimerTextTMP textMM;
    
    private int lastTime = -1;

    public void SetTimeLeft(int time)
    {
        if (time == lastTime)
        {
            return;
        }

        lastTime = time;
        var ss = lastTime % 60;
        var mm = lastTime / 60;

        textSS.SetValue(ss);
        textMM.SetValue(mm);
    }
}