using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Serializable]
    public class EnemySpawnEntry
    {
        public GameObject prefab;

        [Min(1)] public int minPlayerLevel = 1;   // eligible if playerLevel >= this
        [Min(0f)] public float weight = 1f;       // higher = more likely
    }

    [Header("Spawn table (set in Inspector)")]
    [SerializeField] private EnemySpawnEntry[] spawnTable;

    [Header("Spawn settings")]
    [SerializeField] private Transform player;
    [SerializeField] private EnemyDifficultyScaler enemyDifficultyScaler;

    public void SpawnEnemy(Vector3 targetPosition, int playerLevel)
    {
        if (spawnTable == null || spawnTable.Length == 0) return;

        GameObject enemyPrefab = PickEnemyPrefabByLevelAndWeight(playerLevel);
        if (enemyPrefab == null) return;

        var spawnPosition = Utils.Vector3XY(targetPosition, enemyPrefab.transform.position);
        GameObject newSpawn = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        //Set Scaled speed
        var navMeshAgent = newSpawn.GetComponent<UnityEngine.AI.NavMeshAgent>();
        navMeshAgent.speed = enemyDifficultyScaler.ScaleSpeedWithLevel(navMeshAgent.speed, PlayerStats.Instance.currentLevel, 1);
        //Set Scaled health
        var enemyHealth = newSpawn.GetComponent<EnemyHealth>();
        enemyHealth.health = enemyDifficultyScaler.ScaleHealthWithLevel(enemyHealth.health, PlayerStats.Instance.currentLevel, 1);
        //Set Scaled damage
        var enemyAttack = newSpawn.GetComponent<EnemyWeaponController>();
        enemyAttack.damage = enemyDifficultyScaler.ScaleDamageWithLevel(enemyAttack.damage, PlayerStats.Instance.currentLevel, 1);
    }

    private GameObject PickEnemyPrefabByLevelAndWeight(int level)
    {
        // 1) Compute total weight among eligible entries
        float totalWeight = 0f;

        for (int i = 0; i < spawnTable.Length; i++)
        {
            var e = spawnTable[i];
            if (e.prefab == null) continue;
            if (level < e.minPlayerLevel) continue;
            if (e.weight <= 0f) continue;

            totalWeight += e.weight;
        }

        if (totalWeight <= 0f) return null;

        // 2) Weighted pick
        float roll = UnityEngine.Random.value * totalWeight;

        for (int i = 0; i < spawnTable.Length; i++)
        {
            var e = spawnTable[i];
            if (e.prefab == null) continue;
            if (level < e.minPlayerLevel) continue;
            if (e.weight <= 0f) continue;

            roll -= e.weight;
            if (roll <= 0f)
                return e.prefab;
        }
        // Fallback (rare float edge case)
        for (int i = spawnTable.Length - 1; i >= 0; i--)
            if (spawnTable[i].prefab != null && level >= spawnTable[i].minPlayerLevel && spawnTable[i].weight > 0f)
                return spawnTable[i].prefab;

        return null;
    }
}
