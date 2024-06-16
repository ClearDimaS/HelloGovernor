using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private float volume = 0.5f;
    [SerializeField] private AudioClip clip;
    
    public void PlaySound()
    {
        SoundManager.Instance.PlaySound(clip, volume);
    }

    public void SetClip(AudioClip clip)
    {
        this.clip = clip;
    }
}