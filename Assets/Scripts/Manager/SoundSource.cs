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

        // Invoke: ���� ����
        CancelInvoke();
        _audioSource.clip = clip; ;
        _audioSource.volume = soundEffectVolume;
        _audioSource.Play();

        // ���ݾ��� �ٸ� �Ҹ��� ���� 
        _audioSource.pitch = 1f + Random.Range(-soundEffectPitchVariance, soundEffectPitchVariance);

        // Disable�̸��� �޼��带 ��������
        Invoke("Disable", clip.length + 2); 
    }

    public void Disable()
    {
        _audioSource.Stop();
        Destroy(this.gameObject);
    }
}
