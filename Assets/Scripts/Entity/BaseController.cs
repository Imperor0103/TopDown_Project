using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected Rigidbody2D _rigidbody;

    // [SerializeField]를 통해 private도 인스펙터에 공개할 수 있다
    [SerializeField] private SpriteRenderer characterRenderer;
    [SerializeField] private Transform weaponPivot;

    protected Vector2 movementDirection = Vector2.zero; // 이동방향
    public Vector2 MovementDirection { get { return movementDirection; } }

    protected Vector2 lookDirection = Vector2.zero; // 바라보는 방향
    public Vector2 LookDirection { get { return lookDirection; } }

    private Vector2 knockback = Vector2.zero;
    private float knockbackDuration = 0.0f;

    protected AnimationHandler animationHandler;    // 애니메이션
    protected StatHandler statHandler;      // 스탯

    [SerializeField] public WeaponHandler WeaponPrefab;     // 무기 프리팹
    protected WeaponHandler weaponHandler;  // 무기 장착을 위한 핸들러

    protected bool isAttacking;
    private float timeSinceLastAttack = float.MaxValue;


    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        animationHandler = GetComponent<AnimationHandler>();    // 애니메이션
        statHandler = GetComponent<StatHandler>();  // 스탯

        // 무기 장착
        if (WeaponPrefab != null)
            weaponHandler = Instantiate(WeaponPrefab, weaponPivot); // 무기 피벗에 프리팹을 복사해서 생성한다
        else
            weaponHandler = GetComponentInChildren<WeaponHandler>();    // 이미 무기 장착 시, 찾아온다
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        HandleAction(); // 입력처리, 이동에 필요한 데이터 처리
        Rotate(lookDirection);  // 회전

        HandleAttackDelay();
    }

    protected virtual void FixedUpdate()
    {
        Movment(movementDirection); // 이동
        if (knockbackDuration > 0.0f)
        {
            knockbackDuration -= Time.fixedDeltaTime;
        }
    }

    protected virtual void HandleAction()
    {

    }

    private void Movment(Vector2 direction)
    {
        //direction = direction * 5;
        //direction = direction * statHandler.Speed;  // 스탯에 의한 속도 변화

        direction = direction * statHandler.GetStat(StatType.Speed);
        if (knockbackDuration > 0.0f)
        {
            direction *= 0.2f;
            direction += knockback;
        }

        _rigidbody.velocity = direction;

        animationHandler.Move(direction);   // 이동 애니메이션
    }

    private void Rotate(Vector2 direction)
    {
        // Atan2는 y와 x를 받아서 그 사이에 각(rad)을 구한다(arctan 개념이다), 이걸 deg로 바꾼다
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bool isLeft = Mathf.Abs(rotZ) > 90f;    // 절대값이 90도 넘어가면 2,4사분면

        characterRenderer.flipX = isLeft;   // 2,4사분면에 있으면 좌우로 뒤집는다

        if (weaponPivot != null)
        {
            // Quaternion.Euler의 매개변수로 deg값 사용
            weaponPivot.rotation = Quaternion.Euler(0, 0, rotZ);
        }

        // 무기 장착 관련
        weaponHandler?.Rotate(isLeft);  // 무기 회전
    }

    public void ApplyKnockback(Transform other, float power, float duration)
    {
        knockbackDuration = duration;
        knockback = -(other.position - transform.position).normalized * power;
        // 벡터의 뺼셈이다 A-B: B가 A를 바라보는 벡터
    }

    private void HandleAttackDelay()
    {
        if (weaponHandler == null)
            return;

        // 공격 딜레이
        if (timeSinceLastAttack <= weaponHandler.Delay)
        {
            timeSinceLastAttack += Time.deltaTime;
        }

        // 딜레이가 다 되었으면 공격
        if (isAttacking && timeSinceLastAttack > weaponHandler.Delay)
        {
            timeSinceLastAttack = 0;
            Attack();
        }
    }

    protected virtual void Attack()
    {
        if (lookDirection != Vector2.zero)
            weaponHandler?.Attack();
    }

    // BaseController는 PlayerController 또는 EnemyController로부터 누가 죽었는지 정보를 받는다(처리는 ResourceController에서)
    public virtual void Death()
    {
        _rigidbody.velocity = Vector3.zero;

        // 하위에 있는 모든 스프라이트를 찾아온다
        foreach (SpriteRenderer renderer in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            Color color = renderer.color;
            color.a = 0.3f; // 알파값 수정
            renderer.color = color;
        }


        foreach (Behaviour component in transform.GetComponentsInChildren<Behaviour>())
        {
            /// 코드가 동작하지 않도록 모두 끄는 작업
            component.enabled = false;
        }

        Destroy(gameObject, 2f);
    }
}
