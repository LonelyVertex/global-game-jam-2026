using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaskProjectileManager : MonoBehaviour
{
    public int ExtraDamage { get; set; }
    public int ExtraSplashDamage { get; set; }
    public int ExtraShootingCount { get; set; }
    public int ExtraProjectileCount
    {
        get => _extraProjectileCount;
        set
        {
            _extraProjectileCount = value;

            if (_maskInfo.spawnType == MaskInfo.ESpawnType.orbital)
            {
                SpawnOrbitalProjectiles();
            }
        }
    }
    public int ExtraBounces { get; set; }
    public int ExtraOrbitalSpeed { get; set; }

    private int _extraProjectileCount;

    private MaskInfo _maskInfo;
    private float _lastProjectile;

    private readonly List<Transform> _orbitingProjectiles = new();

    private PlayerController _playerController;

    public int Damage => _maskInfo.damage + ExtraDamage;
    public int SplashDamage => _maskInfo.splashDamage + ExtraSplashDamage;
    private float Cooldown => _maskInfo.cooldown / (1 + ExtraShootingCount);
    private int ProjectileCount => _maskInfo.projectileCount + ExtraProjectileCount;
    public int Bounces => _maskInfo.projectileBounce + ExtraBounces;
    private float OrbitalSpeed => _maskInfo.orbitalSpeed + ExtraOrbitalSpeed;

    public void SetMaskInfo(MaskInfo maskInfo)
    {
        _maskInfo = maskInfo;
        _playerController = GameManager.Instance.Player.GetComponent<PlayerController>();

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
        if (_lastProjectile + PlayerStats.Instance.ScaleCooldown(Cooldown) <= Time.time)
        {
            SpawnProjectiles();
            _lastProjectile = Time.time;
        }
    }

    private void UpdateOrbit()
    {
        // Rotate around the player and follow
        foreach (var orbitingProjectile in _orbitingProjectiles)
        {
            var orbitalSpeed = PlayerStats.Instance.ScaleOrbitalSpeed(OrbitalSpeed);
            orbitingProjectile.RotateAround(transform.position, Vector3.up, orbitalSpeed * Time.deltaTime);
        }
    }


    #region Shooting

    private void SpawnProjectiles()
    {
        bool spawned = false;

        if (_maskInfo.spawnType == MaskInfo.ESpawnType.frontal)
        {
            spawned = SpawnProjectilesFrontal();
        }
        else if (_maskInfo.spawnType == MaskInfo.ESpawnType.melee)
        {
            spawned = SpawnProjectilesMelee();
        }
        else
        {
            spawned = SpawnProjectilesTargetOrDirectional();
        }

        if (spawned && _maskInfo.shootSound)
        {
            SoundManager.Instance.PlaySound(_maskInfo.shootSound);
        }
    }

    private bool SpawnProjectilesFrontal()
    {
        int projectileCount = PlayerStats.Instance.ScaleProjectileCount(ProjectileCount);
        float angleStep = Mathf.Min(_maskInfo.frontalSpreadAngle, 360f / ProjectileCount);
        float spreadAngle = angleStep * ProjectileCount;
        float startingAngle = -spreadAngle / 2;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = startingAngle + (i * angleStep);
            Quaternion rotation = Quaternion.Euler(0, angle, 0) *
                                  Quaternion.LookRotation(_playerController.playerModel.forward);

            var spawnPoint = transform.position + (rotation * Vector3.forward * _maskInfo.projectileSpawnRadius);
            var spawnPosition = Utils.Vector3XY(spawnPoint, _maskInfo.projectilePrefab.transform.position);

            var projectileObject =
                Instantiate(_maskInfo.projectilePrefab, spawnPosition, rotation);
            var projectile = projectileObject.GetComponent<Projectile>();
            projectile.SetMaskInfo(_maskInfo, this);
        }

        return projectileCount > 0;
    }

    private bool SpawnProjectilesMelee()
    {
        var spawnPosition = Utils.Vector3XY(transform.position, _maskInfo.projectilePrefab.transform.position);
        Quaternion rotation = Quaternion.LookRotation(_playerController.playerModel.forward);
        var projectileObject = Instantiate(_maskInfo.projectilePrefab, spawnPosition, rotation);
        var projectile = projectileObject.GetComponent<Projectile>();
        projectile.SetMaskInfo(_maskInfo, this);

        return true;
    }

    private bool SpawnProjectilesTargetOrDirectional()
    {
        int projectileCount = PlayerStats.Instance.ScaleProjectileCount(ProjectileCount);
        var enemies = FindEnemiesInRange(projectileCount);

        bool anyEnemies = false;

        foreach (var e in enemies)
        {
            anyEnemies = true;

            var direction = (Utils.Vector3XY(e.position, transform.position) - transform.position).normalized;
            var spawnPoint = transform.position + (direction * _maskInfo.projectileSpawnRadius);
            var spawnPosition = Utils.Vector3XY(spawnPoint, _maskInfo.projectilePrefab.transform.position);

            var projectileObject =
                Instantiate(_maskInfo.projectilePrefab, spawnPosition, Quaternion.LookRotation(direction));
            var projectile = projectileObject.GetComponent<Projectile>();
            projectile.SetMaskInfo(_maskInfo, this);
            if (_maskInfo.spawnType == MaskInfo.ESpawnType.target)
            {
                projectile.SetTarget(e.transform);
            }
        }

        return anyEnemies;
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

        float angleStep = 360f / ProjectileCount;

        for (int i = 0; i < ProjectileCount; i++)
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

            var projectile = projectileObject.GetComponent<Projectile>();
            projectile.SetMaskInfo(_maskInfo, this);

            _orbitingProjectiles.Add(projectileObject.transform);
        }
    }

    #endregion
}
