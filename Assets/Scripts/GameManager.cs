using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private MaskSelectionPanel maskSelectionPanel;
    [SerializeField] private SkillSelectionPanel skillSelectionPanel;

    [Header("Prefabs")]
    [SerializeField] private GameObject maskProjectileManagerPrefab;

    [Header("Level Generator")]
    [SerializeField] private LevelGenerator levelGenerator;

    [Header("Player")]
    [SerializeField] private GameObject player;

    [Header("Enemies")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnRadius;
    [SerializeField] private float spawnInterval;
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("Mask Box")]
    [SerializeField] private MaskBox maskBoxPrefab;
    [SerializeField] private float firstMaskRadius;
    [SerializeField] private float secondMaskRadius;

    [Header("Spawn Settings")]
    [SerializeField]
    private LayerMask obstacleMask;
    [SerializeField] private float spawnClearanceRadius = 0.3f;
    [SerializeField] private int maxSpawnAttempts = 30;

    [Header("Game State")]
    public float totalTime = 0f;
    public int totalKills = 0;

    public static GameManager Instance { get; private set; }

    public GameObject Player => player;

    private float _lastSpawn;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Start()
    {
        if (levelGenerator)
        {
            yield return StartGame();
        }
    }

    private IEnumerator StartGame()
    {
        yield return levelGenerator.Generate();

        player.SetActive(true);
        SpawnMaskBoxes();
        totalTime = 0f;
        totalKills = 0;
    }

    private void SpawnMaskBoxes()
    {
        SpawnMaskBox(player.transform.position, firstMaskRadius);
        SpawnMaskBox(player.transform.position, secondMaskRadius);
    }

    private void SpawnMaskBox(Vector3 spawnPos, float radius)
    {
        if (TryGetSpawnPositionOnCircle(spawnPos, radius, out var spawnPosition))
        {
            var spawnPositionAdjusted = Utils.Vector3XY(spawnPosition, maskBoxPrefab.transform.position);
            Instantiate(maskBoxPrefab, spawnPositionAdjusted, Quaternion.identity);
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;

        if (Time.time - _lastSpawn >= spawnInterval)
        {
            SpawnEnemy();
            _lastSpawn = Time.time;
        }
        totalTime += Time.deltaTime;
    }

    private void SpawnEnemy()
    {
        if (enemySpawner==null ) return;
        var spawnPosition = TryGetSpawnPositionOnCircle(player.transform.position, spawnRadius, out var spawnPos);
        if (!spawnPosition) return;
        enemySpawner.SpawnEnemy(spawnPos, PlayerStats.Instance.currentLevel);
    }

    public void ShowMaskSelection()
    {
        maskSelectionPanel.GenerateOptions();
        maskSelectionPanel.gameObject.SetActive(true);

        Time.timeScale = 0;
    }

    public void ShowSkillSelection()
    {
        skillSelectionPanel.GenerateOptions();
        skillSelectionPanel.gameObject.SetActive(true);

        Time.timeScale = 0;
    }

    public void EquipMask(MaskInfo maskInfo, EMaskEquipMode equipMode)
    {
        PlayerStats.Instance.EquipMask(maskInfo);

        EquipMaskManager(maskInfo, equipMode);

        MasksProvider.Instance.EquipMask(maskInfo);
        PlayerMasksController.Instance.EquipMask(maskInfo);

        maskSelectionPanel.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    private void EquipMaskManager(MaskInfo maskInfo, EMaskEquipMode equipMode)
    {
        if (equipMode == EMaskEquipMode.newMask)
        {
            var playerTransform = PlayerStats.Instance.transform;
            var managerObject = Instantiate(maskProjectileManagerPrefab, playerTransform);
            var maskProjectileManager = managerObject.GetComponent<MaskProjectileManager>();
            maskProjectileManager.SetMaskInfo(maskInfo);

            PlayerStats.Instance.AddMaskManager(maskInfo, maskProjectileManager);

            return;
        }

        var manager = PlayerStats.Instance.GetMaskManager(maskInfo);
        if (!manager) return;

        switch (equipMode)
        {
            case EMaskEquipMode.damage:
                manager.ExtraDamage += maskInfo.damage;
                manager.ExtraSplashDamage += maskInfo.splashDamage;
                break;

            case EMaskEquipMode.cooldown:
                manager.ExtraShootingCount += 1;
                break;

            case EMaskEquipMode.projectileCount:
                manager.ExtraProjectileCount += maskInfo.projectileCount;
                break;

            case EMaskEquipMode.projectileBounce:
                manager.ExtraBounces += Mathf.Max(1, maskInfo.projectileBounce);
                break;

            case EMaskEquipMode.orbitalSpeed:
                manager.ExtraOrbitalSpeed += manager.ExtraOrbitalSpeed;
                break;
        }
    }

    public void TakeSkill(SkillInfo skillInfo)
    {
        PlayerStats.Instance.TakeSkill(skillInfo);

        skillSelectionPanel.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    private bool TryGetSpawnPositionOnCircle(
        Vector3 center,
        float radius,
        out Vector3 spawnPos)
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            spawnPos = GetSpawnPositionOnCircle(center, radius);

            // if anything in obstacleMask overlaps our clearance sphere, it's invalid
            bool blocked = Physics.CheckSphere(
                spawnPos,
                spawnClearanceRadius,
                obstacleMask,
                QueryTriggerInteraction.Ignore);

            if (!blocked)
            {
                return true;
            }
        }

        spawnPos = default;
        return false;
    }

    private Vector3 GetSpawnPositionOnCircle(Vector3 center, float radius)
    {
        Vector2 offset2D = Random.insideUnitCircle.normalized * radius;
        return center + new Vector3(offset2D.x, 0f, offset2D.y);
    }
}
