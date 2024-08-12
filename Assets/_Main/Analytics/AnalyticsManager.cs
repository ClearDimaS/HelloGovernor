using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsManager : Singleton<AnalyticsManager>
{
    protected override void OnCreated()
    {
        base.OnCreated();
        //AppMetrica.Instance.RequestTrackingAuthorization (status => {  });
    }

    public void StartLevel(int level)
    {
        //AM_Manager.Instance.TrackLevelStart(level);
        AM_Manager.Instance.TrackLevelStart(level);
    }

    public void WinLevel(int level)
    {
       // AM_Manager.Instance.TrackLevelSuccess(level);
       AM_Manager.Instance.TrackLevelSuccess(level);
    }

    public void LoseLevel(int level)
    {
       // AM_Manager.Instance.TrackLevelFail(level);
       AM_Manager.Instance.TrackLevelFail(level);
    }
}
