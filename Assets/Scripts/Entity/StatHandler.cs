using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatHandler : MonoBehaviour
{
    /// Range: Min과 Max 사이에서 제한
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

    // 일시적인 스탯인지?, 얼마나 지속되는지?
    public void ModifyStat(StatType statType, float amount, bool isPermanent = true, float duration = 0)
    {
        if (!currentStats.ContainsKey(statType)) return;

        currentStats[statType] += amount;

        /// isPermanent가 true이면 RemoveStatAfterDuration이 호출되어 스탯 증감 효과가 삭제 
        if (!isPermanent)
        {
            StartCoroutine(RemoveStatAfterDuration(statType, amount, duration));
        }
    }

    // 추가된 스탯을 일정 시간 후에 제거
    private IEnumerator RemoveStatAfterDuration(StatType statType, float amount, float duration)
    {
        yield return new WaitForSeconds(duration);
        currentStats[statType] -= amount;
    }


}
