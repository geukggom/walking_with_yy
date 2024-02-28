using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviourSingleton<SoundManager>
{
    private AudioSource _bgmAudioSource;
    private AudioSource _soundAudioSource;

    private void Awake()
    {
        _bgmAudioSource = GetComponent<AudioSource>();
        _soundAudioSource = this.AddComponent<AudioSource>();
        
        _bgmAudioSource.loop = true;
        _soundAudioSource.loop = false;
    }

    public void PlayBgm(string soundPath)
    {
        _bgmAudioSource.Stop();
        
        var audioClip = Resources.Load<AudioClip>(soundPath);
        if (audioClip == null)
        {
            Debug.LogError($"Not Found AudioClip : {soundPath}");
            return;
        }
        _bgmAudioSource.clip = audioClip;
        _bgmAudioSource.Play();
    }

    public void StopBgm()
    {
        _bgmAudioSource.Stop();
    }

    public void PlaySound(string soundPath)
    {
        _soundAudioSource.Stop();
        
        var audioClip = Resources.Load<AudioClip>(soundPath);
        if (audioClip == null)
        {
            Debug.LogError($"Not Found AudioClip : {soundPath}");
            return;
        }
        _soundAudioSource.clip = audioClip;
        _soundAudioSource.Play();
    }
}
