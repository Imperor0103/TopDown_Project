using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �÷��̾��� HP ����
public class ResourceController : MonoBehaviour
{
    [SerializeField] private float healthChangeDelay = .5f; // �����⵿ֱ�� ��������

    private BaseController baseController;
    private StatHandler statHandler;
    private AnimationHandler animationHandler;

    private float timeSinceLastChange = float.MaxValue; // ��ȭ�� ���� �ð� �����Ͽ�, �����ð��Ŀ� �ٽ� ��ȭ�� �޴´�

    public float CurrentHealth { get; private set; }
    //public float MaxHealth => statHandler.Health;
    public float MaxHealth => statHandler.GetStat(StatType.Health);


    public AudioClip damageClip;   // �ǰ� ���� 

    /// <summary>
    /// delegate�� Ȱ���� �̺�Ʈ ȣ��
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
                animationHandler.InvincibilityEnd();    // ���� ����
            }
        }
    }

    // ������ ����
    public bool ChangeHealth(float change)
    {
        // ���������϶��� ������ �ȹ޴´�
        if (change == 0 || timeSinceLastChange < healthChangeDelay)
        {
            return false;
        }

        timeSinceLastChange = 0f;   // ������ �޾����� �ð��� 0���� �ٲپ� ��� ��������
        CurrentHealth += change;    // +: ȸ��, -: ������
        CurrentHealth = CurrentHealth > MaxHealth ? MaxHealth : CurrentHealth;  // ü�� Max
        CurrentHealth = CurrentHealth < 0 ? 0 : CurrentHealth;  // ü�� Min

        // HP ��ȭ���� ������ �� ȣ��
        /// OnChangeHealth delegate�� ����� �޼��尡 �ִٸ�, CurrentHealth�� MaxHealth �޼��带 �Ű������� �����ؼ� ȣ���Ѵ�
        OnChangeHealth?.Invoke(CurrentHealth, MaxHealth);

        if (change < 0)
        {
            animationHandler.Damage();  // ������ �޴´�

            // �ǰ� ����
            if (damageClip != null)
                SoundManager.PlayClip(damageClip);
        }

        if (CurrentHealth <= 0f)
        {
            Death();
        }

        return true;
    }

    // PlayerController, EnemyController ���� ���ó���ϸ� ResourceController���� �������
    private void Death()
    {
        // BaseController�� ���� �׾����� �˰� �ִ�
        baseController.Death();
    }

    /// <summary>
    /// float 2���� �޴� Action�� �Ű������� �޴´�
    /// </summary>
    /// <param name="action"></param>
    public void AddHealthChangeEvent(Action<float, float> action)
    {
        OnChangeHealth += action;
        /// ������ OnChangeHealth delegate�� ����� action��
        /// HP�� ��ȭ�ɶ����� ChangeHealth���� 
        /// OnChangeHealth?.Invoke(CurrentHealth, MaxHealth); ���� ȣ��ȴ�
    }
    public void RemoveHealthChangeEvent(Action<float, float> action)
    {
        OnChangeHealth -= action;
        /// ������ OnChangeHealth delegate�� ����� action��
        /// HP�� ��ȭ�ɶ����� ChangeHealth���� 
        /// OnChangeHealth?.Invoke(CurrentHealth, MaxHealth); ���� ȣ��ȴ�
    }
}
