using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerController player { get; private set; }
    private ResourceController _playerResourceController;

    [SerializeField] private int currentWaveIndex = 0;

    private EnemyManager enemyManager;

    // HomeUI ����
    private UIManager uiManager;
    public static bool isFirstLoading = true;   /// static�̹Ƿ�, Scene�� �ٽ� �ε��ϴ��� ���� �����ִ�

    // �ó׸ӽ�
    private CameraShake cameraShake;

    // ��������
    [SerializeField] private int currentStageIndex = 0;

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
        StartNextWave();
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
        StartNextWave();    // ���� ���̺� ����
    }

    public void GameOver()
    {
        enemyManager.StopWave();

        uiManager.SetGameOver();
    }

    public void StartStage()
    {
        StageInfo stageInfo = GetStageInfo(currentStageIndex);

        if (stageInfo == null)
        {
            Debug.Log("�������� ������ �����ϴ�.");
            return;
        }

        uiManager.ChangeWave(currentStageIndex + 1);

        enemyManager.StartStage(stageInfo.waves[currentWaveIndex]);
    }

    public void StartNextWaveInStage()
    {
        StageInfo stageInfo = GetStageInfo(currentStageIndex);
        if (stageInfo.waves.Length - 1 > currentWaveIndex)
        {
            currentWaveIndex++;
            StartStage();   // ���� ���̺� ����
        }
        else
        {
            CompleteStage();    // ���� ��������
        }
    }

    public void CompleteStage()
    {
        currentStageIndex++;
        currentWaveIndex = 0;
        StartStage();
    }


    private StageInfo GetStageInfo(int stageKey)
    {
        foreach (var stage in StageData.Stages)
        {
            if (stage.stageKey == stageKey) return stage;
        }
        return null;
    }


    // �׽�Ʈ �ڵ�   
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        StartGame();
    //    }
    //}
}
