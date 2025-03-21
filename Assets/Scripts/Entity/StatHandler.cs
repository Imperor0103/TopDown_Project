using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatHandler : MonoBehaviour
{
    /// Range: Min�� Max ���̿��� ����
    //[Range(1, 100)][SerializeField] private int health = 10;

    //public int Health
    //{
    //    get => health;
    //    set => health = Mathf.Clamp(value, 0, 100);
    //}

    //[Range(1f, 20f)][SerializeField] private float speed = 10;
    //public float Speed
    //{
    //    get => speed;
    //    set => speed = Mathf.Clamp(value, 0, 20);
    //}
    public StatData statData;
    private Dictionary<StatType, float> currentStats = new Dictionary<StatType, float>();

    private void Awake()
    {
        InitializeStats();
    }

    private void InitializeStats()
    {
        foreach (StatEntry entry in statData.stats)
        {
            currentStats[entry.statType] = entry.baseValue;
        }
    }

    public float GetStat(StatType statType)
    {
        return currentStats.ContainsKey(statType) ? currentStats[statType] : 0;
    }

    // �Ͻ����� ��������?, �󸶳� ���ӵǴ���?
    public void ModifyStat(StatType statType, float amount, bool isPermanent = true, float duration = 0)
    {
        if (!currentStats.ContainsKey(statType)) return;

        currentStats[statType] += amount;

        /// isPermanent�� true�̸� RemoveStatAfterDuration�� ȣ��Ǿ� ���� ���� ȿ���� ���� 
        if (!isPermanent)
        {
            StartCoroutine(RemoveStatAfterDuration(statType, amount, duration));
        }
    }

    // �߰��� ������ ���� �ð� �Ŀ� ����
    private IEnumerator RemoveStatAfterDuration(StatType statType, float amount, float duration)
    {
        yield return new WaitForSeconds(duration);
        currentStats[statType] -= amount;
    }


}
