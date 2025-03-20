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

    // HomeUI 관련
    private UIManager uiManager;
    public static bool isFirstLoading = true;   /// static이므로, Scene을 다시 로드하더라도 값이 남아있다

    // 시네머신
    private CameraShake cameraShake;

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
        StartNextWave();
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
        StartNextWave();    // 다음 웨이브 시작
    }

    public void GameOver()
    {
        enemyManager.StopWave();

        uiManager.SetGameOver();
    }


    // 테스트 코드   
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        StartGame();
    //    }
    //}
}
