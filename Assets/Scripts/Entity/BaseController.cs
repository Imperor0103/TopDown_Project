using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected Rigidbody2D _rigidbody;

    // [SerializeField]�� ���� private�� �ν����Ϳ� ������ �� �ִ�
    [SerializeField] private SpriteRenderer characterRenderer;
    [SerializeField] private Transform weaponPivot;

    protected Vector2 movementDirection = Vector2.zero; // �̵�����
    public Vector2 MovementDirection { get { return movementDirection; } }

    protected Vector2 lookDirection = Vector2.zero; // �ٶ󺸴� ����
    public Vector2 LookDirection { get { return lookDirection; } }

    private Vector2 knockback = Vector2.zero;
    private float knockbackDuration = 0.0f;

    protected AnimationHandler animationHandler;    // �ִϸ��̼�
    protected StatHandler statHandler;      // ����

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        animationHandler = GetComponent<AnimationHandler>();    // �ִϸ��̼�
        statHandler = GetComponent<StatHandler>();  // ����
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        HandleAction(); // �Է�ó��, �̵��� �ʿ��� ������ ó��
        Rotate(lookDirection);  // ȸ��
    }

    protected virtual void FixedUpdate()
    {
        Movment(movementDirection); // �̵�
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
        direction = direction * statHandler.Speed;  // ���ȿ� ���� �ӵ� ��ȭ
        if (knockbackDuration > 0.0f)
        {
            direction *= 0.2f;
            direction += knockback;
        }

        _rigidbody.velocity = direction;

        animationHandler.Move(direction);   // �̵� �ִϸ��̼�
    }

    private void Rotate(Vector2 direction)
    {
        // Atan2�� y�� x�� �޾Ƽ� �� ���̿� ��(rad)�� ���Ѵ�(arctan �����̴�), �̰� deg�� �ٲ۴�
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bool isLeft = Mathf.Abs(rotZ) > 90f;    // ���밪�� 90�� �Ѿ�� 2,4��и�

        characterRenderer.flipX = isLeft;   // 2,4��и鿡 ������ �¿�� �����´�

        if (weaponPivot != null)
        {
            // Quaternion.Euler�� �Ű������� deg�� ���
            weaponPivot.rotation = Quaternion.Euler(0, 0, rotZ);
        }
    }

    public void ApplyKnockback(Transform other, float power, float duration)
    {
        knockbackDuration = duration;
        knockback = -(other.position - transform.position).normalized * power;
        // ������ �E���̴� A-B: B�� A�� �ٶ󺸴� ����
    }
}
