using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerController player { get; private set; }
    private ResourceController _playerResourceController;

    // ���� wave, stage ����(���� �� �����ص� �̾ �ϰ� �����)
    [SerializeField] private int currentStageIndex = 0;
    [SerializeField] private int currentWaveIndex = 0;

    private EnemyManager enemyManager;

    // HomeUI ����
    private UIManager uiManager;
    public static bool isFirstLoading = true;   /// static�̹Ƿ�, Scene�� �ٽ� �ε��ϴ��� ���� �����ִ�

    // �ó׸ӽ�
    private CameraShake cameraShake;


    // ���� �������� ����
    private StageInstance currentStageInstance;

    private void Awake()
    {
        instance = this;
        player = FindObjectOfType<PlayerController>();
        player.Init(this);

        uiManager = FindObjectOfType<UIManager>();


        /// HP�� ��ȭ�� ����� UIManager�� ChangePlayerHP�� ȣ���Ѵ�
        _playerResourceController = player.GetComponent<ResourceController>();
        _playerResourceController.RemoveHealthChangeEvent(uiManager.ChangePlayerHP);    // Ȥ�ø� �浹������ ���� ���� Remove
        _playerResourceController.AddHealthChangeEvent(uiManager.ChangePlayerHP);


        enemyManager = GetComponentInChildren<EnemyManager>();
        enemyManager.Init(this);

        // �ó׸ӽ��� �̿��� ī�޶� ����
        cameraShake = FindObjectOfType<CameraShake>();
        MainCameraShake();  // ó�� �����ϸ� ��鸰��
    }

    public void MainCameraShake()
    {
        cameraShake.ShakeCamera(1, 1, 1);
    }

    private void Start()
    {
        // ������ "ù��°" �����ϸ� �ٷ� ���� ������ �� �ְ� ����Button�� ���;� �Ѵ�
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

        // ���� wave�� �ε��� �÷��ְ�
        enemyManager.StartWave(1 + currentWaveIndex / 5);

        // wave�� ��ü�Ѵ�
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
        StageSaveManager.ClearSavedStage();         // ���ӿ����ϸ� ���������� ������
    }

    private void LoadOrStartNewStage()
    {
        // ���̺굥����
        StageInstance savedInstance = StageSaveManager.LoadStageInstance();

        if (savedInstance != null)
        {
            currentStageInstance = savedInstance;   // ���̺굥���͸� �ε�
        }
        else
        {
            currentStageInstance = new StageInstance(0, 0); // ���� ����
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
            Debug.Log("�������� ������ �����ϴ�.");
            StageSaveManager.ClearSavedStage();
            currentStageInstance = null;
            return;
        }
        // stageInstance�� ���� �÷����ϴ� stage ������ ����
        stageInstance.SetStageInfo(stageInfo);
        // ���� 
        uiManager.ChangeWave(currentStageIndex + 1);
        enemyManager.StartStage(currentStageInstance);
        /// wave�� �ϳ� �����Ҷ����� ����
        StageSaveManager.SaveStageInstance(currentStageInstance);
    }

    public void StartNextWaveInStage()
    {
        //StageInfo stageInfo = GetStageInfo(currentStageIndex);
        //if (stageInfo.waves.Length - 1 > currentWaveIndex)
        if (currentStageInstance.CheckEndOfWave())
        {
            currentStageInstance.currentWave++;
            StartStage(currentStageInstance);   // ���� ���̺� ����
        }
        else
        {
            CompleteStage();    // ���� ��������
        }
    }

    public void CompleteStage()
    {
        // ������ �����Ѱ� ������
        StageSaveManager.ClearSavedStage();

        if (currentStageInstance == null)
            return;

        //currentStageIndex++;
        //currentWaveIndex = 0;

        // �������� �ϳ� ����
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
