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

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        HandleAction(); // 입력처리, 이동에 필요한 데이터 처리
        Rotate(lookDirection);  // 회전
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
        direction = direction * 5;
        if (knockbackDuration > 0.0f)
        {
            direction *= 0.2f;
            direction += knockback;
        }

        _rigidbody.velocity = direction;
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
    }

    public void ApplyKnockback(Transform other, float power, float duration)
    {
        knockbackDuration = duration;
        knockback = -(other.position - transform.position).normalized * power;
        // 벡터의 뺼셈이다 A-B: B가 A를 바라보는 벡터
    }
}
