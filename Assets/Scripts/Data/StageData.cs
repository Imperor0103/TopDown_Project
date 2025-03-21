using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 외부에서 불러오지 않고 스크립트를 통해 stage 정보 정리
/// </summary>
/// 
[System.Serializable]
public class StageInfo
{
    public int stageKey;    // stage를 구분할 수 있는 key값
    public WaveData[] waves;    

    // 하나의 stage에 여러 wave가 존재
    public StageInfo(int stageKey, WaveData[] waves)
    {
        this.stageKey = stageKey;
        this.waves = waves;
    }
}

[System.Serializable]
public class WaveData
{
    public MonsterSpawnData[] monsters;
    // 보스가 포함되어 있는지 아닌지
    public bool hasBoss;
    public string bossType;

    public WaveData(MonsterSpawnData[] monsters, bool hasBoss, string bossType)
    {
        this.monsters = monsters;
        this.hasBoss = hasBoss;
        this.bossType = bossType;
    }
}

[System.Serializable]
public class MonsterSpawnData
{
    public string monsterType;
    public int spawnCount;

    // 어떤 몬스터를 얼마나 만들어야하는가
    public MonsterSpawnData(string monsterType, int spawnCount)
    {
        this.monsterType = monsterType;
        this.spawnCount = spawnCount;
    }
}

public static class StageData
{
    // 스테이지 정보는 수정하지 않는다
    public static readonly StageInfo[] Stages = new StageInfo[]
    {
        // 스테이지 0
        new StageInfo(0, new WaveData[]
        {
            // 총 3개의 wave가 존재
            new WaveData(new MonsterSpawnData[]
            {
                new MonsterSpawnData("Goblin",1),
            }
            ,false,""), // false: 보스없음

            new WaveData(new MonsterSpawnData[]
            {
                new MonsterSpawnData("Goblin", 3),
            }
            ,false,""),

            new WaveData(new MonsterSpawnData[]
            {
                new MonsterSpawnData("Goblin",2),
                new MonsterSpawnData("Goblin",2),
                new MonsterSpawnData("Goblin",2),
            }
            ,true,"Orc_Shaman"),    // 보스: Orc_Shaman
        }
        ),

        // 스테이지 1
        new StageInfo(1, new WaveData[]
        {
            new WaveData(new MonsterSpawnData[]
            {
                new MonsterSpawnData("Goblin",5),
            }
            ,false,""),

            new WaveData(new MonsterSpawnData[]
            {
                new MonsterSpawnData("Goblin", 10),
            }
            ,false,""),

            new WaveData(new MonsterSpawnData[]
            {
                new MonsterSpawnData("Goblin",10),
                new MonsterSpawnData("Goblin",10),
                new MonsterSpawnData("Goblin",10),
            }
            ,true,"Orc_Shaman"),
        }
        ),
    };

}
