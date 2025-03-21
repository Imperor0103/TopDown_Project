using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���������� �����ϴ� ����: StageInstance
/// ����Ƽ JsonUtility�� ����ϰ� ���� public, [System.Serializable] ���� �ʼ�
/// </summary>
[System.Serializable]
public class StageInstance
{
    // stage �����Ҷ� �����ؾ��ϴ� ����
    // ������ �����ߴ°�?
    public int stageKey;     
    public int currentWave;         
    public StageInfo currentStageInfo;  // ������� ������ ������������ ������ ��´�

    // �������� ����
    public StageInstance(int stageKey, int currentWave)
    {
        this.stageKey = stageKey;
        this.currentWave = currentWave;
    }

    public void SetStageInfo(StageInfo stageInfo)
    {
        currentStageInfo = stageInfo;
    }

    public bool CheckEndOfWave()
    {
        // stage ������ ����
        if (currentStageInfo == null) return false;

        // stage�� ������ wave���� ���ڰ� ũ��
        if (currentWave >= currentStageInfo.waves.Length - 1) return false;

        return true;
    }
}

// �������� ����, �ε�
public class StageSaveManager
{
    private const string SaveKey = "StageInstance";

    // ����Ƽ���� �����ϴ� JsonUtility�� �̿�
    public static void SaveStageInstance(StageInstance instance)
    {
        // JsonUtility.ToJson: Ŭ������ Json���� �ٲ۴�
        string json = JsonUtility.ToJson(instance);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public static StageInstance LoadStageInstance()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            // �ٽ� Ŭ������ ��ȯ
            return JsonUtility.FromJson<StageInstance>(json);
        }
        return null;    // key�� ���ٸ� null
    }

    // ������ ����
    public static void ClearSavedStage()
    {
        // �������
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
    }
}
