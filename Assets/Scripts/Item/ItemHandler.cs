using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    // SO(Scriptable Object)�� ������ �ְ� �Ѵ�
    [SerializeField] private ItemData itemData;
    
    // ������ �����͸� �����´�(������Ƽ)
    public ItemData ItemData => itemData;
}
