using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BaseController
{
    private EnemyManager enemyManager;
    private Transform target;

    [SerializeField] private float followRange = 15f;

    public void Init(EnemyManager enemyManager, Transform target)
    {
        this.enemyManager = enemyManager;
        this.target = target;
    }

    protected float DistanceToTarget()
    {
        return Vector3.Distance(transform.position, target.position);
    }

    // 플레이어는 이동, 공격을 어떻게 할지 키 입력을 받고 결정했지만
    // 몬스터는 이동을 할지 공격을 할지 스스로 판단
    protected override void HandleAction()
    {
        base.HandleAction();

        if (weaponHandler == null || target == null)
        {
            if (!movementDirection.Equals(Vector2.zero)) movementDirection = Vector2.zero;
            return;
        }

        float distance = DistanceToTarget();
        Vector2 direction = DirectionToTarget();

        isAttacking = false;
        if (distance <= followRange)
        {
            // 이동
            lookDirection = direction;

            if (distance <= weaponHandler.AttackRange)
            {
                // 공격
                int layerMaskTarget = weaponHandler.target;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, weaponHandler.AttackRange * 1.5f,
                    (1 << LayerMask.NameToLayer("Level")) | layerMaskTarget);   // OR처리이므로 1 << LayerMask.NameToLayer("Level")와 layerMaskTarget의 합이다
                                                                                // 1 << LayerMask.NameToLayer("Level"): 특정 레이어 "Level"을 비트 마스크로 변환합니다.


                // 충돌체가 있다면, 충돌해서 처리해야하는 layer가 맞는지 확인한다
                // layer가 "Level"의 layer이면 공격하지 않는다
                if (hit.collider != null && layerMaskTarget == (layerMaskTarget | (1 << hit.collider.gameObject.layer)))
                {
                    isAttacking = true;
                }

                movementDirection = Vector2.zero;
                return;
            }

            movementDirection = direction;
        }

    }

    protected Vector2 DirectionToTarget()
    {
        return (target.position - transform.position).normalized;
    }
}
