using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    // SO(Scriptable Object)만 가지고 있게 한다
    [SerializeField] private ItemData itemData;
    
    // 아이템 데이터를 가져온다(프로퍼티)
    public ItemData ItemData => itemData;
}
