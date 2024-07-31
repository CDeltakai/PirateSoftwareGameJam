using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    PlayerController player;

    [SerializeField] int maxEnemies = 9;
    [SerializeField] int spawnGroupCount = 3;

    [SerializeField] int maxEnemyHP = 5;
    [SerializeField] int minEnemyHP = 1;

    [SerializeField] GameObject enemyParent;
    [SerializeField] GameObject meleeEnemyPrefab;
    [SerializeField] GameObject rangedEnemyPrefab;

    StageManager stageManager;
    TurnManager turnManager;

    [SerializeField] int spawnCooldown = 6;
    [SerializeField] int currentCooldown = 0;

    [SerializeField] List<StageEntity> enemies = new();

    void Start()
    {
        stageManager = StageManager.Instance;
        turnManager = TurnManager.Instance;
        player = GameManager.Instance.PlayerRef;

        AttemptSpawnWave();
        turnManager.OnNextTurn += AttemptSpawnWave;
    }

    void AttemptSpawnWave()
    {
        CleanEnemiesList();
        if (currentCooldown > 0)
        {
            currentCooldown--;
            return;
        }

        int enemiesToSpawn = Mathf.Min(maxEnemies - enemies.Count, spawnGroupCount);
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
        }
    }

    void CleanEnemiesList()
    {
        enemies.RemoveAll(e => e == null);
    }

    void SpawnEnemy()
    {
        int retryCount = 0;
        Vector3Int spawnTile = stageManager.GetRandomValidTile().localCoordinates;
        while (spawnTile == null || Vector3.Distance(spawnTile, player.transform.position) < 5f)
        {
            if (retryCount > 30) { return; }

            spawnTile = stageManager.GetRandomValidTile().localCoordinates;
            if (spawnTile == null)
            {
                retryCount++;
                continue;
            }
        }

        if(spawnTile == null) { return; }  

        GameObject enemyPrefab = Random.Range(0, 2) == 0 ? meleeEnemyPrefab : rangedEnemyPrefab;
        GameObject enemy = Instantiate(enemyPrefab, spawnTile, Quaternion.identity, enemyParent.transform);

        enemy.GetComponent<StageEntity>().CurrentHP = Random.Range(minEnemyHP, maxEnemyHP);

        enemies.Add(enemy.GetComponent<StageEntity>());

        currentCooldown = spawnCooldown;
    }

}
