using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaskProjectileManager : MonoBehaviour
{
    [SerializeField] private MaskInfo maskInfo;
    [SerializeField] private LayerMask enemyLayerMask;

    private float _lastProjectile;

    private readonly List<Transform> _orbitingProjectiles = new();

    private void Start()
    {
        if (maskInfo.spawnType == MaskInfo.ESpawnType.orbital)
        {
            SpawnOrbitalProjectiles();
        }
        else
        {
            _lastProjectile = Time.time;
        }
    }

    private void Update()
    {
        if (maskInfo.spawnType == MaskInfo.ESpawnType.orbital)
        {
            UpdateOrbit();
        }
        else
        {
            UpdateShooting();
        }
    }

    private void UpdateShooting()
    {
        if (_lastProjectile + PlayerStats.Instance.ScaleCooldown(maskInfo.cooldown) <= Time.time)
        {
            SpawnProjectile();
            _lastProjectile = Time.time;
        }
    }

    private void UpdateOrbit()
    {
        // Rotate around the player and follow
        foreach (var orbitingProjectile in _orbitingProjectiles)
        {
            orbitingProjectile.RotateAround(transform.position, Vector3.up, maskInfo.orbitalSpeed * Time.deltaTime);
        }
    }


    #region Shooting

    private void SpawnProjectile()
    {
        int projectileCount = PlayerStats.Instance.ScaleProjectileCount(maskInfo.projectileCount);
        var enemies = FindEnemiesInRange(projectileCount);

        foreach (var e in enemies)
        {
            var direction = GetSpawnDirection(e);
            var spawnPosition = transform.position + (direction * maskInfo.projectileSpawnRadius);

            var projectileObject = Instantiate(maskInfo.projectilePrefab, spawnPosition, Quaternion.LookRotation(direction));
            var projectile = projectileObject.GetComponent<Projectile>();

            if (maskInfo.spawnType == MaskInfo.ESpawnType.target)
            {
                projectile.SetTarget(e.transform);
            }
        }
    }

    private Vector3 GetSpawnDirection(Transform target)
    {
        // TODO use model transform for forward direction
        return maskInfo.spawnType == MaskInfo.ESpawnType.frontal
            ? transform.forward
            : (target.position - transform.position).normalized;
    }

    private IEnumerable<Transform> FindEnemiesInRange(int enemyCount)
    {
        return Utils.FindEnemiesInRangeSorted(transform.position, maskInfo.range)
            .Take(enemyCount);
    }

    #endregion

    #region Orbit

    private void SpawnOrbitalProjectiles()
    {
        foreach (var orbitingProjectile in _orbitingProjectiles)
        {
            Destroy(orbitingProjectile.gameObject);
        }
        _orbitingProjectiles.Clear();

        float angleStep = 360f / maskInfo.projectileCount;

        for (int i = 0; i < maskInfo.projectileCount; i++)
        {
            float angle = i * angleStep;
            float radians = angle * Mathf.Deg2Rad;

            var spawnPosition = new Vector3(
                transform.position.x + (maskInfo.projectileSpawnRadius * Mathf.Cos(radians)),
                transform.position.y,
                transform.position.z + (maskInfo.projectileSpawnRadius * Mathf.Sin(radians))
            );

            var projectileObject = Instantiate(maskInfo.projectilePrefab, transform);
            projectileObject.transform.position = spawnPosition;
            _orbitingProjectiles.Add(projectileObject.transform);
        }
    }

    #endregion
}
