using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fixed Virtual Camera�� �پ �켱������ �����Ѵ�
/// Virtual Camera�� ���� Collider�� �÷��̾ �浹�� 
/// ��ũ��Ʈ�� ���� Fixed Virtual Camera�� �켱������ ������, ī�޶� �ٲٴ� ���̴�
/// </summary>
public class VirtualCameraController : MonoBehaviour
{
    CinemachineVirtualCamera vcam;

    public int currentPriority = 5; // ���� �켱����
    public int activeProiority = 20;    

    private void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �÷��̾��ΰ�?
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
