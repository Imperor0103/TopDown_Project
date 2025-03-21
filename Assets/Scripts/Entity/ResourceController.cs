using System;
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
    //public float MaxHealth => statHandler.Health;
    public float MaxHealth => statHandler.GetStat(StatType.Health);


    public AudioClip damageClip;   // 피격 사운드 

    /// <summary>
    /// delegate를 활용한 이벤트 호출
    /// </summary>
    private Action<float, float> OnChangeHealth;

    private void Awake()
    {
        statHandler = GetComponent<StatHandler>();
        animationHandler = GetComponent<AnimationHandler>();
        baseController = GetComponent<BaseController>();
    }

    private void Start()
    {
        //CurrentHealth = statHandler.Health;
        CurrentHealth = statHandler.GetStat(StatType.Health);
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

        // HP 변화량이 생겼을 때 호출
        /// OnChangeHealth delegate에 연결된 메서드가 있다면, CurrentHealth와 MaxHealth 메서드를 매개변수로 전달해서 호출한다
        OnChangeHealth?.Invoke(CurrentHealth, MaxHealth);

        if (change < 0)
        {
            animationHandler.Damage();  // 데미지 받는다

            // 피격 사운드
            if (damageClip != null)
                SoundManager.PlayClip(damageClip);
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

    /// <summary>
    /// float 2개를 받는 Action을 매개변수로 받는다
    /// </summary>
    /// <param name="action"></param>
    public void AddHealthChangeEvent(Action<float, float> action)
    {
        OnChangeHealth += action;
        /// 위에서 OnChangeHealth delegate에 등록한 action은
        /// HP가 변화될때마다 ChangeHealth에서 
        /// OnChangeHealth?.Invoke(CurrentHealth, MaxHealth); 으로 호출된다
    }
    public void RemoveHealthChangeEvent(Action<float, float> action)
    {
        OnChangeHealth -= action;
        /// 위에서 OnChangeHealth delegate에 등록한 action은
        /// HP가 변화될때마다 ChangeHealth에서 
        /// OnChangeHealth?.Invoke(CurrentHealth, MaxHealth); 으로 호출된다
    }
}
