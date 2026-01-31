using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaskProjectileManager : MonoBehaviour
{
    [SerializeField] private MaskInfo maskInfo;
    [SerializeField] private LayerMask enemyLayerMask;

    private float _lastProjectile;
    private readonly Collider[] _enemiesInRange = new Collider[100];

    private void Start()
    {
        _lastProjectile = Time.time;
    }

    private void Update()
    {
        if (_lastProjectile + maskInfo.cooldown <= Time.time)
        {
            SpawnProjectile();
            _lastProjectile = Time.time;
        }
    }

    private void SpawnProjectile()
    {
        var enemies = FindEnemiesInRange(maskInfo.projectileCount);

        foreach (var e in enemies)
        {
            var direction = (e.position - transform.position).normalized;
            var spawnPosition = transform.position + (direction * maskInfo.projectileSpawnRadius);

            var projectileObject = Instantiate(maskInfo.projectilePrefab, spawnPosition, Quaternion.LookRotation(direction));
            var projectile = projectileObject.GetComponent<Projectile>();
            projectile.Damage = maskInfo.damage;

            if (maskInfo.projectileTargeted)
            {
                projectile.SetTarget(e.transform);
            }
        }
    }

    private IEnumerable<Transform> FindEnemiesInRange(int enemyCount)
    {
        int enemies = Physics.OverlapSphereNonAlloc(transform.position, maskInfo.range, _enemiesInRange, enemyLayerMask);

        if (enemies == 0)
        {
            return Enumerable.Empty<Transform>();
        }

        return _enemiesInRange
            .Where(e => e)
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .Take(enemyCount)
            .Select(e => e.transform);
    }
}
