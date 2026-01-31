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
        SpawnFirstMaskBox();
        totalTime = 0f;
        totalKills = 0;
    }

    private void SpawnFirstMaskBox()
    {
        if (TryGetSpawnPositionOnCircle(player.transform.position, firstMaskRadius, out var spawnPos))
        {
            var spawnPosition = Utils.Vector3XY(spawnPos, maskBoxPrefab.transform.position);
            Instantiate(maskBoxPrefab, spawnPosition, Quaternion.identity);
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

    public void EquipMask(MaskInfo maskInfo)
    {
        var playerTransform = PlayerStats.Instance.transform;

        PlayerStats.Instance.EquipMask(maskInfo);

        var managerObject = Instantiate(maskProjectileManagerPrefab, playerTransform);
        managerObject.GetComponent<MaskProjectileManager>().SetMaskInfo(maskInfo);

        MasksProvider.Instance.EquipMask(maskInfo);
        PlayerMasksController.Instance.EquipMask(maskInfo);

        maskSelectionPanel.gameObject.SetActive(false);
        Time.timeScale = 1;
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
