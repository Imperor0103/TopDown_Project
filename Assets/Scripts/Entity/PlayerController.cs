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

    // Start로 Player를 초기화 한 것을 GameManager가 초기화될 때 Player를 초기화하는 방식으로 바꾸었다
    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        camera = Camera.main;
    }

    /// <summary>
    /// 유니티 Input System을 적용하기 전의 Move, Look, Fire 
    /// </summary>
    protected override void HandleAction()
    {
        /// OnMove로 이동
        //float horizontal = Input.GetAxisRaw("Horizontal");
        //float vertical = Input.GetAxisRaw("Vertical");
        //movementDirection = new Vector2(horizontal, vertical).normalized;

        /// OnLook으로 이동
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

        /// OnFire로 이동
        // BaseController(부모 클래스)에서 사용할 isAttacking 변수 값 설정
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
            /// item.isTemporary이 true일때만 일시적으로 생성한다
            /// ModifyStat 내부에 isPermanent가 true로 들어가야 아이템이 삭제된다(즉, 아이템이 일시적으로 생성된다)
            /// 여기서는 isTemporary를 대입했으므로 false가 들어가야 함
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
    /// 유니티의 Input System을 적용
    /// OnMove, OnLOok, OnFire
    /// 
    /// 기존의 입력을 사용하지 않고
    /// InputValue을 매개변수로 전달하여, 유니티에서 제공한 InputValue를 사용한다
    /// 
    /// 각 키가 입력되면 위의 세 함수가 호출되어 처리한다
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
        // EventSystem은 Canvas 추가하면 자동생성되는 그 EventSystem이다
        // UI를 클릭했는지 판단한다
        // 여기서는 UI를 클릭하면 공격하지 않게 하기 위해 추가
        if (EventSystem.current.IsPointerOverGameObject())  
            return;

        isAttacking = inputValue.isPressed;
    }
}