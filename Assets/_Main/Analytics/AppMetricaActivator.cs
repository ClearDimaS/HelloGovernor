using Io.AppMetrica;
using UnityEngine;

public static class AppMetricaActivator
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Activate() {
        AppMetrica.Activate(new AppMetricaConfig("4c2159fd-9467-40de-ba5f-db03ddc4487c") {
            FirstActivationAsUpdate = !IsFirstLaunch(),
            LocationTracking = true,
        });
        PlayerPrefs.SetInt("app_metrica_launched", 11);
    }

    private static bool IsFirstLaunch() {
        return PlayerPrefs.GetInt("app_metrica_launched") != 11;
    }
}