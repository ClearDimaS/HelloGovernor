using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[Serializable]
public class SerializedAudioData
{
    public AudioClip clip;
    public float minPause;
    public float volume;
    [HideInInspector] public float lastPLayTime;
}

public class SoundManager : MonoBehaviour
{
    [Inject] private CacheManager cacheManager;
    
    [SerializeField] private AudioSource source;
    [SerializeField] private SerializedAudioData spendMoney;
    [SerializeField] private SerializedAudioData purchase;
    [SerializeField] private SerializedAudioData getMoney;
    [SerializeField] private SerializedAudioData takeItem;
    [SerializeField] private SerializedAudioData repair;
    [SerializeField] private SerializedAudioData wishDone;
    [SerializeField] private SerializedAudioData taskComplete;
    [SerializeField] private SerializedAudioData[] clicks;
    
    [field: SerializeField] public AudioClip PlayerFootstepsSound;
    
    public void PlaySound(AudioClip clip, float volume)
    {
        PlayClip(clip, volume);
    }

    public void PlayPurchase()
    {
        PlaySoundInternal(purchase);
    }
    
    public void PlaySpendMoney()
    {
        PlaySoundInternal(spendMoney);
    }
    
    public void PlayerGetMoney()
    {
        PlaySoundInternal(getMoney);
    }
    
    public void PlayerTakeItem()
    {
        PlaySoundInternal(takeItem);
    }
    
    public void PlayerRepair()
    {
        PlaySoundInternal(repair);
    }
    
    public void WishDone()
    {
        PlaySoundInternal(wishDone);
    }
    
    public void TaskComplete()
    {
        PlaySoundInternal(taskComplete);
    }

    private void PlaySoundInternal(SerializedAudioData soundData)
    {
        if (!cacheManager.IsSoundOn)
        {
            return;
        }
        if (Time.time - soundData.lastPLayTime > soundData.minPause)
        {
            soundData.lastPLayTime = Time.time;
            PlayClip(soundData.clip, soundData.volume);
        }
    }

    private void PlayClip(AudioClip clip, float volume)
    {
        source.PlayOneShot(clip, volumeScale:volume);
    }

    public void Click(int getInstanceID)
    {
        var clickID = Mathf.Abs(getInstanceID);
        var data = clicks[clickID % clicks.Length];
        PlaySoundInternal(data);
    }
}
