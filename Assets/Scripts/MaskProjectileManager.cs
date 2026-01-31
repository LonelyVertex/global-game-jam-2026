using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaskProjectileManager : MonoBehaviour
{
    private MaskInfo _maskInfo;
    private float _lastProjectile;

    private readonly List<Transform> _orbitingProjectiles = new();

    public void SetMaskInfo(MaskInfo maskInfo)
    {
        _maskInfo = maskInfo;

        if (_maskInfo.spawnType == MaskInfo.ESpawnType.orbital)
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
        if (!_maskInfo) return;
        if (PlayerStats.Instance.IsDead()) return;

        if (_maskInfo.spawnType == MaskInfo.ESpawnType.orbital)
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
        if (_lastProjectile + PlayerStats.Instance.ScaleCooldown(_maskInfo.cooldown) <= Time.time)
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
            orbitingProjectile.RotateAround(transform.position, Vector3.up, _maskInfo.orbitalSpeed * Time.deltaTime);
        }
    }


    #region Shooting

    private void SpawnProjectile()
    {
        int projectileCount = PlayerStats.Instance.ScaleProjectileCount(_maskInfo.projectileCount);
        var enemies = FindEnemiesInRange(projectileCount);

        foreach (var e in enemies)
        {
            var direction = GetSpawnDirection(e);
            var spawnPoint = transform.position + (direction * _maskInfo.projectileSpawnRadius);
            var spawnPosition = Utils.Vector3XY(spawnPoint, _maskInfo.projectilePrefab.transform.position);

            Debug.Log(spawnPosition);

            var projectileObject =
                Instantiate(_maskInfo.projectilePrefab, spawnPosition, Quaternion.LookRotation(direction));
            var projectile = projectileObject.GetComponent<Projectile>();
            projectile.SetMaskInfo(_maskInfo);
            if (_maskInfo.spawnType == MaskInfo.ESpawnType.target)
            {
                projectile.SetTarget(e.transform);
            }
        }
    }

    private Vector3 GetSpawnDirection(Transform target)
    {
        // TODO use model transform for forward direction
        return _maskInfo.spawnType == MaskInfo.ESpawnType.frontal
            ? transform.forward
            : (Utils.Vector3XY(target.position, transform.position) - transform.position).normalized;
    }

    private IEnumerable<Transform> FindEnemiesInRange(int enemyCount)
    {
        return Utils.FindEnemiesInRangeSorted(transform.position, _maskInfo.range)
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

        float angleStep = 360f / _maskInfo.projectileCount;

        for (int i = 0; i < _maskInfo.projectileCount; i++)
        {
            float angle = i * angleStep;
            float radians = angle * Mathf.Deg2Rad;

            var spawnPosition = new Vector3(
                transform.position.x + (_maskInfo.projectileSpawnRadius * Mathf.Cos(radians)),
                transform.position.y,
                transform.position.z + (_maskInfo.projectileSpawnRadius * Mathf.Sin(radians))
            );

            var projectileObject = Instantiate(_maskInfo.projectilePrefab, transform);
            projectileObject.transform.position = spawnPosition;
            _orbitingProjectiles.Add(projectileObject.transform);
        }
    }

    #endregion
}
