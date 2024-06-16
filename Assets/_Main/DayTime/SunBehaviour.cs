using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;


public class SunBehaviour : MonoBehaviour
{
    [Inject] private DayTimeManager dayTimeManager;

    [SerializeField] private Light light;
    [SerializeField] private Vector3 extraRot;

    private void Update()
    {
        var curT = dayTimeManager.GetDaytT();

        var fwd = Quaternion.Euler(0, 0, curT * 360f) * Vector3.up;
        transform.rotation = Quaternion.LookRotation(fwd, Vector3.forward) * Quaternion.Euler(extraRot);
        
        DayTimeData currentData = null;
        DayTimeData nextData = null;

        var datas = dayTimeManager.CycledDatas;
        for (var i = 0; i < datas.Count - 1; i++)
        {
            var data = datas[i];
            if (data.startT <= curT)
            {
                currentData = data;
                nextData = datas[i + 1];
            }
        }

        var t = Mathf.InverseLerp(currentData.startT, nextData.startT, curT);
        light.shadowStrength = Mathf.Lerp(currentData.shadows, nextData.shadows, t);
        light.color = Color.Lerp(currentData.sunColor, nextData.sunColor, t);
        light.intensity = Mathf.Lerp(currentData.intensity, nextData.intensity, t);
    }
}
