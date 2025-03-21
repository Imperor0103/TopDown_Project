using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private Coroutine waveRoutine;

    [SerializeField]
    private List<GameObject> enemyPrefabs; // 생성할 적 프리팹 리스트
    private Dictionary<string, GameObject> enemyPrefabDic;  // 리스트에 있는걸 딕셔너리에 옮긴다

    [SerializeField]
    private List<Rect> spawnAreas; // 적을 생성할 영역 리스트

    [SerializeField]
    private Color gizmoColor = new Color(1, 0, 0, 0.3f); // 기즈모 색상

    private List<EnemyController> activeEnemies = new List<EnemyController>(); // 현재 활성화된 적들

    private bool enemySpawnComplite;

    [SerializeField] private float timeBetweenSpawns = 0.2f;
    [SerializeField] private float timeBetweenWaves = 1f;

    GameManager gameManager;

    /// <summary>
    /// 생성한 아이템을 알고 있다
    /// </summary>
    [SerializeField] private List<GameObject> itemPrefabs;

    // GameManager가 Player 뿐 아니라 EnemyManager도 컨트롤한다
    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;

        enemyPrefabDic = new Dictionary<string, GameObject>();
        foreach (GameObject prefab in enemyPrefabs)
        {
            enemyPrefabDic[prefab.name] = prefab;
        }
    }

    public void StartWave(int waveCount)
    {
        if (waveCount <= 0)
        {
            gameManager.EndOfWave();    // 웨이브가 끝나면, 다음 웨이브 시작
            return;
        }

        if (waveRoutine != null)
            StopCoroutine(waveRoutine);
        waveRoutine = StartCoroutine(SpawnWave(waveCount));
    }

    public void StopWave()
    {
        StopAllCoroutines();
    }

    // 이거 코루틴안쓰고 while, for문 쓰면 코드가 멈춰버린다...
    private IEnumerator SpawnWave(int waveCount)
    {
        enemySpawnComplite = false;
        yield return new WaitForSeconds(timeBetweenWaves);  // wave 시작할때 일정시간 대기

        for (int i = 0; i < waveCount; i++)
        {
            yield return new WaitForSeconds(timeBetweenSpawns); // 일정시간 기다리고
            SpawnRandomEnemy(); // 몬스터 생성한다
        }

        enemySpawnComplite = true;
    }

    // 매개변수 default 값을 이용하여 기존의 코드 재사용성을 높인다
    private void SpawnRandomEnemy(string prefabName = null)
    {
        if (enemyPrefabs.Count == 0 || spawnAreas.Count == 0)
        {
            Debug.LogWarning("Enemy Prefabs 또는 Spawn Areas가 설정되지 않았습니다.");
            return;
        }

        GameObject randomPrefab;
        if (prefabName == null)
        {
            /// 랜덤한 적 프리팹 선택
            randomPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        }
        else
        {
            randomPrefab = enemyPrefabDic[prefabName];
        }

        /// 랜덤한 영역 선택
        Rect randomArea = spawnAreas[Random.Range(0, spawnAreas.Count)];

        /// Rect 영역 내부의 랜덤 위치 계산
        Vector2 randomPosition = new Vector2(
            Random.Range(randomArea.xMin, randomArea.xMax),
            Random.Range(randomArea.yMin, randomArea.yMax)
        );
        /// 적 생성 및 리스트에 추가
        GameObject spawnedEnemy = Instantiate(randomPrefab, new Vector3(randomPosition.x, randomPosition.y), Quaternion.identity);
        EnemyController enemyController = spawnedEnemy.GetComponent<EnemyController>();


        // RemoveEnemyOnDeath에서 모든 적이 제거가되면 Wave가 끝났다
        // 이를 EnemyManager가 알고 있어야한다
        // EnemyControll를 초기화
        enemyController.Init(this, gameManager.player.transform);

        activeEnemies.Add(enemyController);
    }

    // 기즈모(개발을 위한 아이콘)를 그려 영역을 시각화 (선택된 경우에만 표시)
    private void OnDrawGizmosSelected()
    {
        if (spawnAreas == null) return;

        Gizmos.color = gizmoColor;
        foreach (var area in spawnAreas)
        {
            Vector3 center = new Vector3(area.x + area.width / 2, area.y + area.height / 2);
            Vector3 size = new Vector3(area.width, area.height);
            Gizmos.DrawCube(center, size);
        }
    }

    public void RemoveEnemyOnDeath(EnemyController enemy)
    {
        // 죽은 Enemy는 리스트에서 제거
        activeEnemies.Remove(enemy);

        /// 아이템을 확률 생성하고싶다면 여기서 랜덤을 추가하면 된다
        CreateRandomItem(enemy.transform.position);

        // EnemySpawn이 완료가 되었고, 남은 Enemy가 없다면 해당 Wave는 완료
        /// 이 작업을 하려면 EnemyController가 EnemyManager를 알고 있어야 한다
        if (enemySpawnComplite && activeEnemies.Count == 0)
            gameManager.EndOfWave();
    }

    public void CreateRandomItem(Vector3 position)
    {
        GameObject item = Instantiate(itemPrefabs[Random.Range(0, itemPrefabs.Count)], position, Quaternion.identity);
    }

    public void StartStage(StageInstance stageInstance)
    {
        if (waveRoutine != null)
            StopCoroutine(waveRoutine);

        waveRoutine = StartCoroutine(SpawnStart(stageInstance));
    }

    private IEnumerator SpawnStart(StageInstance stageInstance)
    {
        enemySpawnComplite = false;
        yield return new WaitForSeconds(timeBetweenWaves);

        WaveData waveData = stageInstance.currentStageInfo.waves[stageInstance.currentWave];

        for (int i = 0; i < waveData.monsters.Length; i++)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);

            MonsterSpawnData monsterSpawnData = waveData.monsters[i];
            for (int j = 0; j < monsterSpawnData.spawnCount; j++)
            {
                SpawnRandomEnemy(monsterSpawnData.monsterType);
            }
        }

        if (waveData.hasBoss)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);

            gameManager.MainCameraShake();
            SpawnRandomEnemy(waveData.bossType);
        }

        enemySpawnComplite = true;
    }
}
