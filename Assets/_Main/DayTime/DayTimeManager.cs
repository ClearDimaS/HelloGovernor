using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class DayTimeData
{
    public float startT;
    public Color sunColor;
    public Color skyColor;
    public float intensity = 1f;
    public float shadows;

    public DayTimeData GetCopy()
    {
        return new DayTimeData()
        {
            startT = this.startT,
            sunColor = this.sunColor,
            skyColor = this.skyColor,
            intensity = this.intensity,
            shadows = this.shadows
        };
    }
}

public class DayTimeManager : MonoBehaviour
{

    [SerializeField] private float dayDuration;
    [SerializeField] private float startT = 0.5f;
    [SerializeField] private DayTimeData[] dayTimes;
    [SerializeField] private float curDuration = 0f;

    public List<DayTimeData> CycledDatas = new();
    
    public bool IsLampsEnabled { get; private set; }
    
    private void Awake()
    {
        curDuration = dayDuration * startT;
        dayTimes[0].startT = 0f;
        CycledDatas = dayTimes.Select(x => x.GetCopy()).ToList();
        var first = dayTimes[0].GetCopy();
        first.startT = 1f;
        CycledDatas.Add(first);
    }

    private void Update()
    {
        curDuration += Time.deltaTime;
        if (curDuration > dayDuration)
        {
            curDuration = 0f;
        }

        var t = GetDaytT();
        IsLampsEnabled = t > .8f || t < 0.2f;
    }

    public float GetDaytT()
    {
        return curDuration / dayDuration;
    }
}
