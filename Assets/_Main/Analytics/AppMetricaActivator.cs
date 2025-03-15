using Io.AppMetrica;
using UnityEngine;

public static class AppMetricaActivator
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Activate() {
        AppMetrica.Activate(new AppMetricaConfig("9db2c504-ce43-4a25-b5f3-22d5400f922f") {
            FirstActivationAsUpdate = !IsFirstLaunch(),
            LocationTracking = true,
        });
        PlayerPrefs.SetInt("app_metrica_launched", 11);
    }

    private static bool IsFirstLaunch() {
        return PlayerPrefs.GetInt("app_metrica_launched") != 11;
    }
}