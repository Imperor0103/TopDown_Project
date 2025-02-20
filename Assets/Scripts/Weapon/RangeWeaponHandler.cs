using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeaponHandler : WeaponHandler
{
    [Header("Ranged Attack Data")]
    [SerializeField] private Transform projectileSpawnPosition;

    [SerializeField] private int bulletIndex;   // 총알의 인덱스를 가져와서 어떤 총을 사용할지 정한다
    public int BulletIndex { get { return bulletIndex; } }

    [SerializeField] private float bulletSize = 1;
    public float BulletSize { get { return bulletSize; } }

    [SerializeField] private float duration;    // 총알의 생존 기간
    public float Duration { get { return duration; } }

    [SerializeField] private float spread;  // 탄 퍼짐의 정도
    public float Spread { get { return spread; } }

    [SerializeField] private int numberofProjectilesPerShot;    // 몇발을 쏠 것인가
    public int NumberofProjectilesPerShot { get { return numberofProjectilesPerShot; } }

    [SerializeField] private float multipleProjectilesAngle;    // 각각의 탄의 퍼짐 정도
    public float MultipleProjectilesAngel { get { return multipleProjectilesAngle; } }

    [SerializeField] private Color projectileColor;     // 총알의 색깔을 다양하게
    public Color ProjectileColor { get { return projectileColor; } }


    private ProjectileManager projectileManager;
    protected override void Start()
    {
        base.Start();
        projectileManager = ProjectileManager.Instance;
    }


    public override void Attack()
    {
        base.Attack();

        float projectilesAngleSpace = multipleProjectilesAngle;  // 각각의 탄의 퍼짐 정도
        int numberOfProjectilesPerShot = numberofProjectilesPerShot;    // 몇발을 쏠 것인가

        // 발사해야하는 최소각도 
        float minAngle = -(numberOfProjectilesPerShot / 2f) * projectilesAngleSpace;


        for (int i = 0; i < numberOfProjectilesPerShot; i++)
        {
            float angle = minAngle + projectilesAngleSpace * i;
            float randomSpread = Random.Range(-spread, spread); // 랜덤의 탄 퍼짐을 적용
            angle += randomSpread;  // 다채로운 각으로 탄이 퍼진다
            CreateProjectile(Controller.LookDirection, angle);
        }
    }

    private void CreateProjectile(Vector2 _lookDirection, float angle)
    {
        // 생성할 때 매니저를 통해 생성하고 발사
        projectileManager.ShootBullet(this, projectileSpawnPosition.position, RotateVector2(_lookDirection, angle));
    }
    private static Vector2 RotateVector2(Vector2 v, float degree)
    {
        return Quaternion.Euler(0, 0, degree) * v;  // 회전수치만큼 벡터(v)를 회전
        /// 주의: 쿼터니언 * 벡터의 교환법칙은 성립하지 않는다
    }
}
