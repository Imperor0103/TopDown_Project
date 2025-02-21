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

        if (change < 0)
        {
            animationHandler.Damage();  // ������ �޴´�
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
}
