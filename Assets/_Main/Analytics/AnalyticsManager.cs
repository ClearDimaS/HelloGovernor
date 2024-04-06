using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsManager : Singleton<AnalyticsManager>
{
    protected override void OnCreated()
    {
        base.OnCreated();
        GameManager.Instance.levelStartEvent += StartLevel;
        GameManager.Instance.levelSuccessEvent += WinLevel;
        GameManager.Instance.levelFailEvent += LoseLevel;
        AppMetrica.Instance.RequestTrackingAuthorization (status => {  });
    }

    public void StartLevel(int level)
    {
        AM_Manager.Instance.TrackLevelStart(level);
        GA_Manager.Instance.TrackLevelStart(level);
    }

    public void WinLevel(int level)
    {
        AM_Manager.Instance.TrackLevelSuccess(level);
        GA_Manager.Instance.TrackLevelSuccess(level);
    }

    public void LoseLevel(int level)
    {
        AM_Manager.Instance.TrackLevelFail(level);
        GA_Manager.Instance.TrackLevelFail(level);
    }
}
