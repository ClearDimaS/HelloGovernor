using System;
using System.Collections.Generic;
using Io.AppMetrica;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

public class AM_Manager : MonoBehaviour
{
    [Inject] private CacheManager playerDataRepository;
    private float levelStartTime = 0f;

    public void TrackLevelStart(int level)
    {
        levelStartTime = Time.realtimeSinceStartup;
        var @params = GetCommonParams(level);

        var jsonParams = JsonConvert.SerializeObject(@params);
        AppMetrica.ReportEvent($"level_start", jsonParams);
        AppMetrica.SendEventsBuffer();
    }
    
    public void TrackLevelSuccess(int level)
    {
        var @params = GetCommonParams(level);
        @params["time_spent"] = Mathf.RoundToInt(Time.realtimeSinceStartup - levelStartTime);
        
        var jsonParams = JsonConvert.SerializeObject(@params);
        AppMetrica.ReportEvent($"level_complete", jsonParams);
        AppMetrica.SendEventsBuffer();
    }
    
    public void TrackLevelFail(int level, bool quit)
    {
        var @params = GetCommonParams(level);
        @params["time_spent"] = Mathf.RoundToInt(Time.realtimeSinceStartup - levelStartTime);
        @params["reason"] = quit ? "Quit" : "Timer";
        var jsonParams = JsonConvert.SerializeObject(@params);
        AppMetrica.ReportEvent($"level_fail", jsonParams);
    }

    private Dictionary<string, object> GetCommonParams(int level)
    {
        Dictionary<string, object> @params = new();
        @params["level"] = level;
        @params["days_since_reg"] = Mathf.Clamp((DateTime.Now - playerDataRepository.RegistrationDate).Days, 0, 9999999);
        return @params;
    }
}