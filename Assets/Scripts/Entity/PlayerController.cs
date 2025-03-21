using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : BaseController
{
    private GameManager gameManager;
    private Camera camera;

    //protected override void Start()
    //{
    //    base.Start();
    //    camera = Camera.main;
    //}

    // Start�� Player�� �ʱ�ȭ �� ���� GameManager�� �ʱ�ȭ�� �� Player�� �ʱ�ȭ�ϴ� ������� �ٲپ���
    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        camera = Camera.main;
    }

    /// <summary>
    /// ����Ƽ Input System�� �����ϱ� ���� Move, Look, Fire 
    /// </summary>
    protected override void HandleAction()
    {
        /// OnMove�� �̵�
        //float horizontal = Input.GetAxisRaw("Horizontal");
        //float vertical = Input.GetAxisRaw("Vertical");
        //movementDirection = new Vector2(horizontal, vertical).normalized;

        /// OnLook���� �̵�
        //Vector2 mousePosition = Input.mousePosition;
        //Vector2 worldPos = camera.ScreenToWorldPoint(mousePosition);
        //lookDirection = (worldPos - (Vector2)transform.position);

        //if (lookDirection.magnitude < .9f)
        //{
        //    lookDirection = Vector2.zero;
        //}
        //else
        //{
        //    lookDirection = lookDirection.normalized;
        //}

        /// OnFire�� �̵�
        // BaseController(�θ� Ŭ����)���� ����� isAttacking ���� �� ����
        //isAttacking = Input.GetMouseButton(0);
    }

    public override void Death()
    {
        base.Death();
        gameManager.GameOver();
    }

    public void UseItem(ItemData item)
    {
        foreach (StatEntry modifier in item.statModifiers)
        {
            /// item.isTemporary�� true�϶��� �Ͻ������� �����Ѵ�
            /// ModifyStat ���ο� isPermanent�� true�� ���� �������� �����ȴ�(��, �������� �Ͻ������� �����ȴ�)
            /// ���⼭�� isTemporary�� ���������Ƿ� false�� ���� ��
            statHandler.ModifyStat(modifier.statType, modifier.baseValue, !item.isTemporary, item.duration);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<ItemHandler>(out ItemHandler handler))
        {
            if (handler.ItemData == null)
                return;

            UseItem(handler.ItemData);
            Destroy(handler.gameObject);
        }
    }


    /// <summary>
    /// ����Ƽ�� Input System�� ����
    /// OnMove, OnLOok, OnFire
    /// 
    /// ������ �Է��� ������� �ʰ�
    /// InputValue�� �Ű������� �����Ͽ�, ����Ƽ���� ������ InputValue�� ����Ѵ�
    /// 
    /// �� Ű�� �ԷµǸ� ���� �� �Լ��� ȣ��Ǿ� ó���Ѵ�
    /// </summary>
    /// <param name="inputValue"></param>
    void OnMove(InputValue inputValue)
    {
        movementDirection = inputValue.Get<Vector2>();
        movementDirection = movementDirection.normalized;
    }
    void OnLook(InputValue inputValue)
    {
        Vector2 mousePosition = inputValue.Get<Vector2>();
        Vector2 worldPos = camera.ScreenToWorldPoint(mousePosition);
        lookDirection = (worldPos - (Vector2)transform.position);

        if (lookDirection.magnitude < .9f)
        {
            lookDirection = Vector2.zero;
        }
        else
        {
            lookDirection = lookDirection.normalized;
        }
    }
    void OnFire(InputValue inputValue)
    {
        // EventSystem�� Canvas �߰��ϸ� �ڵ������Ǵ� �� EventSystem�̴�
        // UI�� Ŭ���ߴ��� �Ǵ��Ѵ�
        // ���⼭�� UI�� Ŭ���ϸ� �������� �ʰ� �ϱ� ���� �߰�
        if (EventSystem.current.IsPointerOverGameObject())  
            return;

        isAttacking = inputValue.isPressed;
    }
}