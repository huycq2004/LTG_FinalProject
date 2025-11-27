using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Prefab")]
    public GameObject enemyPrefab;

    [Header("Enemies per Wave")]
    public List<int> enemiesPerWave; // e.g. [1,2,3]

    [Header("Doors to Control")]
    public List<Door> doors;

    private int currentWave = 0;
    private int aliveEnemies = 0;
    private bool waveStarted = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("WaveSpawner: Trigger entered by " + other.name);
        if (!other.CompareTag("Player")) return;
        if (waveStarted) return;

        CloseAllDoors();

        waveStarted = true;
        StartWave();
    }

    void StartWave()
    {
        if (currentWave >= enemiesPerWave.Count)
        {
            Debug.Log("All waves done!");
            OpenAllDoors();
            return;
        }

        int enemyCount = enemiesPerWave[currentWave];
        aliveEnemies = enemyCount;

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject e = Instantiate(enemyPrefab, transform.position, transform.rotation);
            OrcController enemy = e.GetComponent<OrcController>();
            enemy.spawner = this;
        }

        Debug.Log("Wave " + (currentWave + 1) + " started with " + enemyCount + " enemies!");
    }

    public void OnEnemyKilled()
    {
        aliveEnemies--;
        Debug.Log("======BattleZone======Enemy killed! " + aliveEnemies + " remaining in wave " + (currentWave + 1));

        if (aliveEnemies <= 0)
        {
            currentWave++;
            StartWave();
        }
    }

    void OpenAllDoors()
    {
        if (doors != null)
        {
            foreach (var door in doors)
            {
                if (door != null)
                    door.OpenDoor();
            }
        }
        else
        {
            Debug.LogWarning("WaveSpawner: No doors assigned to control.");
        }
    }

    void CloseAllDoors()
    {
        if (doors != null)
        {
            foreach (var door in doors)
            {
                if (door != null)
                    door.CloseDoor();
            }
        }
        else
        {
            Debug.LogWarning("WaveSpawner: No doors assigned to control.");
        }
    }
}
