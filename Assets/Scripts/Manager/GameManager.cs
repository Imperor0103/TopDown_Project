using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerController player { get; private set; }
    private ResourceController _playerResourceController;

    // 현재 wave, stage 정보(종료 후 시작해도 이어서 하게 만든다)
    [SerializeField] private int currentStageIndex = 0;
    [SerializeField] private int currentWaveIndex = 0;

    private EnemyManager enemyManager;

    // HomeUI 관련
    private UIManager uiManager;
    public static bool isFirstLoading = true;   /// static이므로, Scene을 다시 로드하더라도 값이 남아있다

    // 시네머신
    private CameraShake cameraShake;


    // 현재 스테이지 정보
    private StageInstance currentStageInstance;

    private void Awake()
    {
        instance = this;
        player = FindObjectOfType<PlayerController>();
        player.Init(this);

        uiManager = FindObjectOfType<UIManager>();


        /// HP의 변화가 생기면 UIManager의 ChangePlayerHP를 호출한다
        _playerResourceController = player.GetComponent<ResourceController>();
        _playerResourceController.RemoveHealthChangeEvent(uiManager.ChangePlayerHP);    // 혹시모를 충돌방지를 위해 먼저 Remove
        _playerResourceController.AddHealthChangeEvent(uiManager.ChangePlayerHP);


        enemyManager = GetComponentInChildren<EnemyManager>();
        enemyManager.Init(this);

        // 시네머신을 이용한 카메라 흔들기
        cameraShake = FindObjectOfType<CameraShake>();
        MainCameraShake();  // 처음 시작하면 흔들린다
    }

    public void MainCameraShake()
    {
        cameraShake.ShakeCamera(1, 1, 1);
    }

    private void Start()
    {
        // 게임을 "첫번째" 시작하면 바로 게임 시작할 수 있게 시작Button이 나와야 한다
        if (!isFirstLoading)
        {
            StartGame();
        }
        else
        {
            isFirstLoading = false;
        }
    }


    public void StartGame()
    {
        uiManager.SetPlayGame();
        // StartNextWave();
        // StartStage();
        LoadOrStartNewStage();
    }

    void StartNextWave()
    {
        currentWaveIndex += 1;

        // 현재 wave의 인덱스 올려주고
        enemyManager.StartWave(1 + currentWaveIndex / 5);

        // wave를 교체한다
        uiManager.ChangeWave(currentWaveIndex);
    }

    public void EndOfWave()
    {
        // StartNextWave();
        StartNextWaveInStage();
    }

    public void GameOver()
    {
        enemyManager.StopWave();
        uiManager.SetGameOver();
        StageSaveManager.ClearSavedStage();         // 게임오버하면 저장정보를 날린다
    }

    private void LoadOrStartNewStage()
    {
        // 세이브데이터
        StageInstance savedInstance = StageSaveManager.LoadStageInstance();

        if (savedInstance != null)
        {
            currentStageInstance = savedInstance;   // 세이브데이터를 로드
        }
        else
        {
            currentStageInstance = new StageInstance(0, 0); // 새로 시작
        }

        StartStage(currentStageInstance);
    }

    public void StartStage(StageInstance stageInstance)
    {
        currentStageIndex = stageInstance.stageKey;
        currentWaveIndex = stageInstance.currentWave;

        StageInfo stageInfo = GetStageInfo(stageInstance.stageKey);

        if (stageInfo == null)
        {
            Debug.Log("스테이지 정보가 없습니다.");
            StageSaveManager.ClearSavedStage();
            currentStageInstance = null;
            return;
        }
        // stageInstance에 지금 플레이하는 stage 정보를 저장
        stageInstance.SetStageInfo(stageInfo);
        // 세팅 
        uiManager.ChangeWave(currentStageIndex + 1);
        enemyManager.StartStage(currentStageInstance);
        /// wave가 하나 시작할때마다 저장
        StageSaveManager.SaveStageInstance(currentStageInstance);
    }

    public void StartNextWaveInStage()
    {
        //StageInfo stageInfo = GetStageInfo(currentStageIndex);
        //if (stageInfo.waves.Length - 1 > currentWaveIndex)
        if (currentStageInstance.CheckEndOfWave())
        {
            currentStageInstance.currentWave++;
            StartStage(currentStageInstance);   // 다음 웨이브 시작
        }
        else
        {
            CompleteStage();    // 다음 스테이지
        }
    }

    public void CompleteStage()
    {
        // 데이터 저장한거 날리고
        StageSaveManager.ClearSavedStage();

        if (currentStageInstance == null)
            return;

        //currentStageIndex++;
        //currentWaveIndex = 0;

        // 스테이지 하나 증가
        currentStageInstance.stageKey += 1;
        currentStageInstance.currentWave = 0;

        StartStage(currentStageInstance);
    }


    private StageInfo GetStageInfo(int stageKey)
    {
        foreach (var stage in StageData.Stages)
        {
            if (stage.stageKey == stageKey) return stage;
        }
        return null;
    }
}
