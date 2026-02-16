using UnityEngine;
using System.Collections.Generic;

public class BattleRoomManager : MonoBehaviour
{
    public List<GameObject> enemies;
    public RunnerAI runner;
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;

    private bool battleStarted = false;
    private bool bossSpawned = false; // 🔒 关键锁

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!battleStarted && other.CompareTag("Player"))
        {
            StartBattle();
        }
    }

    void StartBattle()
    {
        battleStarted = true;
        // 激活小怪
        foreach (var e in enemies) if (e != null) e.SetActive(true);
        // 激活Runner
        if (runner != null) { runner.gameObject.SetActive(true); runner.StartPanic(); }
    }

    void Update()
    {
        // 只有当战斗开始，且Boss还没生出来的时候，才检查
        if (battleStarted && !bossSpawned)
        {
            if (CheckClear())
            {
                SpawnBoss();
            }
        }
    }

    bool CheckClear()
    {
        foreach (var e in enemies) if (e != null) return false;
        return true;
    }

    void SpawnBoss()
    {
        bossSpawned = true; // 🔒 立刻上锁！防止下一帧再生成

        Debug.Log(">> 召唤 Boss！");
        if (bossPrefab != null && bossSpawnPoint != null)
        {
            Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
        }
    }
}