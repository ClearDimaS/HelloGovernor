
using System;
using System.Collections.Generic;
using Io.AppMetrica;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

public static class AppMetricaActivator {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Activate() {
        AppMetrica.Activate(new AppMetricaConfig("4c2159fd-9467-40de-ba5f-db03ddc4487c") {
            FirstActivationAsUpdate = !IsFirstLaunch(),
            LocationTracking = false
        });
        PlayerPrefs.SetInt("app_metrica_launched", 11);
    }

    private static bool IsFirstLaunch() {
        return PlayerPrefs.GetInt("app_metrica_launched") != 11;
    }
}

public class AM_Manager : Singleton<AM_Manager>
{
    [Inject] private PlayerDataRepository playerDataRepository;
    private float levelStartTime = 0f;

    public void TrackLevelStart(int level)
    {
        levelStartTime = Time.time;
        var @params = GetCommonParams(level);

        var jsonParams = JsonConvert.SerializeObject(@params);
        AppMetrica.ReportEvent($"level_start", jsonParams);
        AppMetrica.SendEventsBuffer();
    }
    
    public void TrackLevelSuccess(int level)
    {
        var @params = GetCommonParams(level);
        @params["time_spent"] = Mathf.RoundToInt(Time.time - levelStartTime);
        
        var jsonParams = JsonConvert.SerializeObject(@params);
        AppMetrica.ReportEvent($"level_complete", jsonParams);
        AppMetrica.SendEventsBuffer();
    }
    
    public void TrackLevelFail(int level)
    {
        var @params = GetCommonParams(level);
        @params["time_spent"] = Mathf.RoundToInt(Time.time - levelStartTime);
        
        var jsonParams = JsonConvert.SerializeObject(@params);
        AppMetrica.ReportEvent($"level_fail", jsonParams);
    }

    private Dictionary<string, object> GetCommonParams(int level)
    {
        Dictionary<string, object> @params = new();
        @params["level"] = level;
        @params["days since reg"] = (DateTime.Today - playerDataRepository.GetData().registrationDate).Days;
        return @params;
    }
}

