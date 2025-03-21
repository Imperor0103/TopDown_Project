using System.Collections.Generic;
using UnityEngine;


public enum StatType
{
    Health,
    Speed,
    ProjectileCount,
}

[CreateAssetMenu(fileName = "New StatData", menuName = "Stats/Chracter Stats")]
public class StatData : ScriptableObject
{
    public string characterName;
    public List<StatEntry> stats;
}

[System.Serializable]
public class StatEntry
{
    public StatType statType;   // 어떤 스탯인지? 
    public float baseValue;     // 어떤 값인지?
}