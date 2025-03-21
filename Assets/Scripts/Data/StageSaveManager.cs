using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스테이지를 저장하는 기준: StageInstance
/// 유니티 JsonUtility를 사용하가 위해 public, [System.Serializable] 선언 필수
/// </summary>
[System.Serializable]
public class StageInstance
{
    // stage 저장할때 저장해야하는 정보
    // 어디까지 진행했는가?
    public int stageKey;     
    public int currentWave;         
    public StageInfo currentStageInfo;  // 현재까지 진행한 스테이지들의 정보를 담는다

    // 스테이지 생성
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
        // stage 정보가 없다
        if (currentStageInfo == null) return false;

        // stage의 마지막 wave보다 숫자가 크다
        if (currentWave >= currentStageInfo.waves.Length - 1) return false;

        return true;
    }
}

// 스테이지 저장, 로드
public class StageSaveManager
{
    private const string SaveKey = "StageInstance";

    // 유니티에서 제공하는 JsonUtility를 이용
    public static void SaveStageInstance(StageInstance instance)
    {
        // JsonUtility.ToJson: 클래스를 Json으로 바꾼다
        string json = JsonUtility.ToJson(instance);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public static StageInstance LoadStageInstance()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            // 다시 클래스로 전환
            return JsonUtility.FromJson<StageInstance>(json);
        }
        return null;    // key가 없다면 null
    }

    // 데이터 삭제
    public static void ClearSavedStage()
    {
        // 삭제기능
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
    }
}
