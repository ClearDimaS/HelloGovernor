using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    [SerializeField] private AM_Manager amManager;
    
    public static AnalyticsManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        //AppMetrica.Instance.RequestTrackingAuthorization (status => {  });
        #if UNITY_IOS
                if (Unity.Advertisement.IosSupport.ATTrackingStatusBinding.GetAuthorizationTrackingStatus() ==
                    Unity.Advertisement.IosSupport.ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
                {
                    Unity.Advertisement.IosSupport.ATTrackingStatusBinding.RequestAuthorizationTracking();
                }
#endif
    }

    public void StartLevel(int level)
    {
        amManager.TrackLevelStart(level);
    }

    public void WinLevel(int level)
    {
       amManager.TrackLevelSuccess(level);
    }

    public void LoseLevel(int level, bool quit = false)
    {
       amManager.TrackLevelFail(level, quit);
    }
}
