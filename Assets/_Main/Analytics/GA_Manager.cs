using UnityEngine;
using GameAnalyticsSDK;

public class GA_Manager : Singleton<GA_Manager>, IGameAnalyticsATTListener
{
    void Start()
    {
        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            GameAnalytics.RequestTrackingAuthorization(this);
        }
        else
        {
            GameAnalytics.Initialize();
        }
    }

    public void GameAnalyticsATTListenerNotDetermined()
    {
        GameAnalytics.Initialize();
    }
    public void GameAnalyticsATTListenerRestricted()
    {
        GameAnalytics.Initialize();
    }
    public void GameAnalyticsATTListenerDenied()
    {
        GameAnalytics.Initialize();
    }
    public void GameAnalyticsATTListenerAuthorized()
    {
        GameAnalytics.Initialize();
    }

    public void TrackLevelStart(int level)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, $"level {level}");
    }
    public void TrackLevelFail(int level)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, $"level {level}");
    }
    public void TrackLevelSuccess(int level)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, $"level {level}");
    }
}