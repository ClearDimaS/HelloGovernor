using System;
using System.Collections;
using System.Collections.Generic;
//using Facebook.Unity;
using UnityEngine;
using Zenject;

public class AM_Manager : Singleton<AM_Manager>
{
    [Inject] private PlayerDataRepository playerDataRepository;
    private float levelStartTime = 0f;

    public void TrackLevelStart(int level)
    {
        levelStartTime = Time.time;
        var @params = GetCommonParams(level);
        
        AppMetrica.Instance.ReportEvent($"level_start", @params);
        AppMetrica.Instance.SendEventsBuffer();
    }
    
    public void TrackLevelSuccess(int level)
    {
        var @params = GetCommonParams(level);
        @params["time_spent"] = Mathf.RoundToInt(Time.time - levelStartTime);
        
        AppMetrica.Instance.ReportEvent($"level_complete", @params);
        AppMetrica.Instance.SendEventsBuffer();
    }
    
    public void TrackLevelFail(int level)
    {
        var @params = GetCommonParams(level);
        @params["time_spent"] = Mathf.RoundToInt(Time.time - levelStartTime);
        
        AppMetrica.Instance.ReportEvent($"level_fail", @params);
    }

    private Dictionary<string, object> GetCommonParams(int level)
    {
        Dictionary<string, object> @params = new();
        @params["level"] = level;
        @params["days since reg"] = (DateTime.Today - playerDataRepository.GetData().registrationDate).Days;
        return @params;
    }
}
