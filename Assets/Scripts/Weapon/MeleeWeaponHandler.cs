using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponHandler : WeaponHandler
{
    [Header("Melee Attack Info")]
    public Vector2 collideBoxSize = Vector2.one;    // 근거리 무기 충돌 거리

    protected override void Start()
    {
        base.Start();
        collideBoxSize = collideBoxSize * WeaponSize;   // collider 사이즈 조정
    }

    public override void Attack()
    {
        base.Attack();

        /// 형체를 갖고 있는 raycast
        // origin에는 현재위치에서 바라본 방향으로 이동한 좌표
        // 그 자리에서만 쏠 것이므로 distance는 0
        RaycastHit2D hit = Physics2D.BoxCast(transform.position + (Vector3)Controller.LookDirection * collideBoxSize.x,
            collideBoxSize, 0, Vector2.zero, 0, target);

        if (hit.collider != null)
        {
            // 체력관리는 ResourceController에서 
            ResourceController resourceController = hit.collider.GetComponent<ResourceController>();
            if (resourceController != null)
            {
                resourceController.ChangeHealth(-Power);
                // 넉백이 켜져있다면
                if (IsOnKnockback)
                {
                    // BaseController로 조절
                    BaseController controller = hit.collider.GetComponent<BaseController>();
                    if (controller != null)
                    {
                        controller.ApplyKnockback(transform, KnockbackPower, KnockbackTime);
                    }
                }
            }
        }
    }

    // 근거리 무기의 rotate는 원거리 무기의 rotate와 다르다
    public override void Rotate(bool isLeft)
    {
        // 근거리 무기는 y축으로 회전해야 rotate가 잘 적용된다
        if (isLeft)
            transform.eulerAngles = new Vector3(0, 180, 0);
        else
            transform.eulerAngles = new Vector3(0, 0, 0);
    }

}
