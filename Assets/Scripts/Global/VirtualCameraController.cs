using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fixed Virtual Camera에 붙어서 우선순위를 조절한다
/// Virtual Camera에 붙은 Collider에 플레이어가 충돌시 
/// 스크립트가 붙은 Fixed Virtual Camera의 우선순위를 높여서, 카메라를 바꾸는 것이다
/// </summary>
public class VirtualCameraController : MonoBehaviour
{
    CinemachineVirtualCamera vcam;

    public int currentPriority = 5; // 현재 우선순위
    public int activeProiority = 20;    

    private void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어인가?
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            // 
            vcam.Priority = activeProiority;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            vcam.Priority = currentPriority;
        }
    }
}
