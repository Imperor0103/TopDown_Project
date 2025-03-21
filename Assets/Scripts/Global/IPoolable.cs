using UnityEngine;
using System;

public interface IPoolable
{
    // GameObject�� �����ϴ� Action�� �Ű�������(�Ű����� �ȿ� �Լ�, ���ٰ� ����)
    void Initialize(Action<GameObject> returnAction);
    void OnSpawn();
    void OnDespawn();
}