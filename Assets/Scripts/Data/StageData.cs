using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ܺο��� �ҷ����� �ʰ� ��ũ��Ʈ�� ���� stage ���� ����
/// </summary>
/// 
[System.Serializable]
public class StageInfo
{
    public int stageKey;    // stage�� ������ �� �ִ� key��
    public WaveData[] waves;    

    // �ϳ��� stage�� ���� wave�� ����
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
    // ������ ���ԵǾ� �ִ��� �ƴ���
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

    // � ���͸� �󸶳� �������ϴ°�
    public MonsterSpawnData(string monsterType, int spawnCount)
    {
        this.monsterType = monsterType;
        this.spawnCount = spawnCount;
    }
}

public static class StageData
{
    // �������� ������ �������� �ʴ´�
    public static readonly StageInfo[] Stages = new StageInfo[]
    {
        // �������� 0
        new StageInfo(0, new WaveData[]
        {
            // �� 3���� wave�� ����
            new WaveData(new MonsterSpawnData[]
            {
                new MonsterSpawnData("Goblin",1),
            }
            ,false,""), // false: ��������

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
            ,true,"Orc_Shaman"),    // ����: Orc_Shaman
        }
        ),

        // �������� 1
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
