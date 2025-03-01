using System;
using Lofelt.NiceVibrations;
using UnityEngine;
using Zenject;

[Serializable]
public class VibrationData
{
    public HapticPatterns.PresetType type;
    public float pause = 0.15f;
    [HideInInspector] public float lastTimePlayed;
}
public class VibrationManager : MonoBehaviour
{
    [Inject] private CacheManager cacheManager;
    
    public VibrationData click;
    public VibrationData levelUp;
    public VibrationData grab;
    public VibrationData reward;
    public VibrationData chew;
    
    public void Click()
    {
        PlayVibration(click);
    }

    public void LevelUp()
    {
        PlayVibration(levelUp);
    }

    public void Grab()
    {
        PlayVibration(grab);
    }

    public void Reward()
    {
        PlayVibration(reward);
    }

    public void Chew()
    {
        PlayVibration(chew);
    }
    
    private void PlayVibration(VibrationData data)
    {
        if (!cacheManager.IsVibrationsOn)
        {
            return;
        }
        if (Time.time - data.lastTimePlayed < data.pause)
        {
            return;
        }
        data.lastTimePlayed = Time.time;
        HapticPatterns.PlayPreset(data.type);
    }
}