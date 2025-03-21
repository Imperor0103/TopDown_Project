using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour, IPoolable
{
    [SerializeField] private LayerMask levelCollisionLayer;

    private RangeWeaponHandler rangeWeaponHandler;

    private float currentDuration;
    private Vector2 direction;
    private bool isReady;
    private Transform pivot;

    private Rigidbody2D _rigidbody;
    private SpriteRenderer spriteRenderer;

    public bool fxOnDestory = true; // 삭제될 때 이벤트 출력할건지 확인


    ProjectileManager projectileManager;

    private Action<GameObject> returnToPool;    // 풀에 오브젝트를 보내는 함수를 구독한다



    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        pivot = transform.GetChild(0);
    }

    private void Update()
    {
        if (!isReady)
        {
            return;
        }

        currentDuration += Time.deltaTime;

        if (currentDuration > rangeWeaponHandler.Duration)
        {
            DestroyProjectile(transform.position, false);
        }

        _rigidbody.velocity = direction * rangeWeaponHandler.Speed;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // layer는 2진수의 값이다. 비트시프트 연산하기 편하다
        // collision.gameObject.layer: 충돌한 오브젝트의 layer
        // 이를 시프트연산하여 OR 또는 AND (여기서는 OR) 충돌한것인지 판단한다

        // 벽면과 충돌 -> 화살 삭제
        if (levelCollisionLayer.value == (levelCollisionLayer.value | (1 << collision.gameObject.layer)))   // 시프트연산
        {
            DestroyProjectile(collision.ClosestPoint(transform.position) - direction * .2f, fxOnDestory);
        }

        // target과 충돌 -> 충돌한 오브젝트의 데미지와 넉백 처리
        else if (rangeWeaponHandler.target.value == (rangeWeaponHandler.target.value | (1 << collision.gameObject.layer)))
        {
            // Entity(생명체)들은 ResourceController를 가지고 있어서 
            // ResourceController를 통해 체력 관리
            ResourceController resourceController = collision.GetComponent<ResourceController>();
            if (resourceController != null)
            {
                resourceController.ChangeHealth(-rangeWeaponHandler.Power);
                // KnockBack(뒤로 밀려남)이 켜져있다면, 그것또한 처리한다
                if (rangeWeaponHandler.IsOnKnockback)
                {
                    BaseController controller = collision.GetComponent<BaseController>();
                    if (controller != null)
                    {
                        controller.ApplyKnockback(transform, rangeWeaponHandler.KnockbackPower, rangeWeaponHandler.KnockbackTime);
                    }
                }
            }

            DestroyProjectile(collision.ClosestPoint(transform.position), fxOnDestory);
        }
    }

    public void Init(Vector2 direction, RangeWeaponHandler weaponHandler, ProjectileManager projectileManager)
    {
        this.projectileManager = projectileManager;

        rangeWeaponHandler = weaponHandler;

        this.direction = direction;
        currentDuration = 0;
        transform.localScale = Vector3.one * weaponHandler.BulletSize;
        spriteRenderer.color = weaponHandler.ProjectileColor;

        transform.right = this.direction;
        // transform.right: 이 오브젝트가 가진 transfrom의 오른쪽 방향

        // pivot을 회전시켜야 날라가는 투사체가 제대로 보인다
        if (this.direction.x < 0)
            pivot.localRotation = Quaternion.Euler(180, 0, 0);
        else
            pivot.localRotation = Quaternion.Euler(0, 0, 0);

        isReady = true;
    }
    private void DestroyProjectile(Vector3 position, bool createFx)
    {
        if (createFx)
        {
            projectileManager.CreateImpactParticlesAtPosition(position, rangeWeaponHandler);
        }

        // Destroy(this.gameObject);
        OnDespawn();    // 
    }

    public void Initialize(Action<GameObject> returnAction)
    {
        // returnToPool에 오브젝트를 풀에 집어넣을 함수를 추가한다
        returnToPool = returnAction;
    }

    public void OnSpawn()
    {

    }

    public void OnDespawn()
    {
        // returnToPool에 함수가 있으면 호출하여 오브젝트를 풀에 집어넣는다
        returnToPool?.Invoke(gameObject);
    }


}
