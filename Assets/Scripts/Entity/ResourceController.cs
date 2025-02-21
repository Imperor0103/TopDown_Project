using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어의 HP 관리
public class ResourceController : MonoBehaviour
{
    [SerializeField] private float healthChangeDelay = .5f; // 일정주기동안 무적상태

    private BaseController baseController;
    private StatHandler statHandler;
    private AnimationHandler animationHandler;

    private float timeSinceLastChange = float.MaxValue; // 변화를 가진 시간 저장하여, 일정시간후에 다시 변화를 받는다

    public float CurrentHealth { get; private set; }
    public float MaxHealth => statHandler.Health;

    private void Awake()
    {
        statHandler = GetComponent<StatHandler>();
        animationHandler = GetComponent<AnimationHandler>();
        baseController = GetComponent<BaseController>();
    }

    private void Start()
    {
        CurrentHealth = statHandler.Health;
    }

    private void Update()
    {
        if (timeSinceLastChange < healthChangeDelay)
        {
            timeSinceLastChange += Time.deltaTime;
            if (timeSinceLastChange >= healthChangeDelay)
            {
                animationHandler.InvincibilityEnd();    // 무적 해제
            }
        }
    }

    // 데미지 적용
    public bool ChangeHealth(float change)
    {
        // 무적상태일때는 데미지 안받는다
        if (change == 0 || timeSinceLastChange < healthChangeDelay)
        {
            return false;
        }

        timeSinceLastChange = 0f;   // 데미지 받았으면 시간을 0으로 바꾸어 잠시 무적상태
        CurrentHealth += change;    // +: 회복, -: 데미지
        CurrentHealth = CurrentHealth > MaxHealth ? MaxHealth : CurrentHealth;  // 체력 Max
        CurrentHealth = CurrentHealth < 0 ? 0 : CurrentHealth;  // 체력 Min

        if (change < 0)
        {
            animationHandler.Damage();  // 데미지 받는다
        }

        if (CurrentHealth <= 0f)
        {
            Death();
        }

        return true;
    }

    // PlayerController, EnemyController 에서 사망처리하면 ResourceController에서 사망판정
    private void Death()
    {
        // BaseController는 누가 죽었는지 알고 있다
        baseController.Death();
    }
}
