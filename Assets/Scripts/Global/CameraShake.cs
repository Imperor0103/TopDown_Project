using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CinemachineVirtualCamera에 붙여 카메라의 활동을 제어한다
/// </summary>
public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin perlin;  // virtualCamera에 추가한 noise
    private float shakeTimeRemaining;   // shake 시간

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();

        /// Noise는 virtualCamera를 통해서 가져와야한다
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
        // 값 초기화
        shakeTimeRemaining = 0f;
        perlin.m_AmplitudeGain = 0f;
        perlin.m_FrequencyGain = 0f;
    }
}
