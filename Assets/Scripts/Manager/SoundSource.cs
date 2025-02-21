using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : MonoBehaviour
{
    private AudioSource _audioSource;

    public void Play(AudioClip clip, float soundEffectVolume, float soundEffectPitchVariance)
    {
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();

        // Invoke: 지연 실행
        CancelInvoke();
        _audioSource.clip = clip; ;
        _audioSource.volume = soundEffectVolume;
        _audioSource.Play();

        // 조금씩은 다른 소리를 낸다 
        _audioSource.pitch = 1f + Random.Range(-soundEffectPitchVariance, soundEffectPitchVariance);

        // Disable이름의 메서드를 지연실행
        Invoke("Disable", clip.length + 2); 
    }

    public void Disable()
    {
        _audioSource.Stop();
        Destroy(this.gameObject);
    }
}
