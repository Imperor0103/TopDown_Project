using UnityEngine;
using System;

public interface IPoolable
{
    // GameObject를 리턴하는 Action을 매개변수로(매개변수 안에 함수, 람다가 들어간다)
    void Initialize(Action<GameObject> returnAction);
    void OnSpawn();
    void OnDespawn();
}