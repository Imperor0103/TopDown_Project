using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CinemachineVirtualCamera�� �ٿ� ī�޶��� Ȱ���� �����Ѵ�
/// </summary>
public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin perlin;  // virtualCamera�� �߰��� noise
    private float shakeTimeRemaining;   // shake �ð�

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();

        /// Noise�� virtualCamera�� ���ؼ� �����;��Ѵ�
        perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float duration, float amplitude, float frequency)
    {
        if (shakeTimeRemaining > duration)
            return;

        shakeTimeRemaining = duration;

        perlin.m_AmplitudeGain = amplitude;
        perlin.m_FrequencyGain = frequency;
    }

    void Update()
    {
        if (shakeTimeRemaining > 0)
        {
            shakeTimeRemaining -= Time.deltaTime;
            if (shakeTimeRemaining <= 0f)
            {
                StopShake();
            }
        }
    }

    public void StopShake()
    {
        // �� �ʱ�ȭ
        shakeTimeRemaining = 0f;
        perlin.m_AmplitudeGain = 0f;
        perlin.m_FrequencyGain = 0f;
    }
}
