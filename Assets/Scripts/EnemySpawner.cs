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
        public float weightScaler = 1f;
        public float chanceToBeBoss = 0f; // chance to spawn as boss variant
    }

    [Header("Spawn table (set in Inspector)")]
    [SerializeField] private EnemySpawnEntry[] spawnTable;
    [SerializeField] private GameObject bossPrefab;

    [Header("Spawn settings")]
    [SerializeField] private Transform player;
    [SerializeField] private EnemyDifficultyScaler enemyDifficultyScaler;

    public void SpawnEnemy(Vector3 targetPosition, int playerLevel)
    {
        if (spawnTable == null || spawnTable.Length == 0) return;

        GameObject enemyPrefab = PickEnemyPrefabByLevelAndWeight(playerLevel);
        if (enemyPrefab == null) return;

        var enemyLevel = Mathf.CeilToInt(GameManager.Instance.totalTime / 20f);

        var spawnPosition = Utils.Vector3XY(targetPosition, enemyPrefab.transform.position);
        GameObject newSpawn = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        //Set Scaled speed
        var navMeshAgent = newSpawn.GetComponent<UnityEngine.AI.NavMeshAgent>();
        navMeshAgent.speed = enemyDifficultyScaler.ScaleSpeedWithLevel(navMeshAgent.speed, enemyLevel, 1);
        //Set Scaled health
        var enemyHealth = newSpawn.GetComponent<EnemyHealth>();
        enemyHealth.health = enemyDifficultyScaler.ScaleHealthWithLevel(enemyHealth.health, enemyLevel, 1);
        //Set Scaled damage
        var enemyAttack = newSpawn.GetComponent<EnemyWeaponController>();
        enemyAttack.damage = enemyDifficultyScaler.ScaleDamageWithLevel(enemyAttack.damage, enemyLevel, 1);

        GameManager.Instance.activesEnemies.Add(newSpawn);

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

            //Scaled Weight
            var scaledWeight = e.weight;
            if (e.weightScaler > 1) {
                scaledWeight = e.weight + (level - e.minPlayerLevel) * e.weightScaler;
            }

            totalWeight += scaledWeight;
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

            //Scaled Weight
            var scaledWeight = e.weight;
            if (e.weightScaler > 1) {
                scaledWeight = e.weight + (level - e.minPlayerLevel) * e.weightScaler;
            }
            roll -= scaledWeight;
            if (roll <= 0f)
                return PrefabOrBoss(e.prefab, e.chanceToBeBoss);
        }
        // Fallback (rare float edge case)
        for (int i = spawnTable.Length - 1; i >= 0; i--)
            if (spawnTable[i].prefab != null && level >= spawnTable[i].minPlayerLevel && spawnTable[i].weight > 0f)
                return PrefabOrBoss(spawnTable[i].prefab, spawnTable[i].chanceToBeBoss);

        return null;
    }

    GameObject PrefabOrBoss(GameObject prefab, float chanceToBeBoss)
    {
        if (bossPrefab != null && UnityEngine.Random.value < chanceToBeBoss)
        {
            return bossPrefab;
        }
        return prefab;
    }
}
