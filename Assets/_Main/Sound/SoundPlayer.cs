using UnityEngine;
using Zenject;

public class SoundPlayer : MonoBehaviour
{
    [Inject] private SoundManager soundManager;
    
    [SerializeField] private float volume = 0.5f;
    [SerializeField] private AudioClip clip;
    
    public void PlaySound()
    {
        soundManager.PlaySound(clip, volume);
    }

    public void SetClip(AudioClip clip)
    {
        this.clip = clip;
    }
}