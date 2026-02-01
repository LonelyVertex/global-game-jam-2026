using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private GameObject model;

    private MaskInfo _maskInfo;
    private MaskProjectileManager _maskProjectileManager;
    private Transform _targetTransform;
    private Vector3 _lastTargetPosition;

    private readonly HashSet<GameObject> _hits = new();

    private float _distanceTraveled;

    private int _bounceCount;
    private readonly List<Transform> _bouncedTargets = new();

    private bool _isBeingDestroyed;

    public void SetMaskInfo(MaskInfo maskInfo, MaskProjectileManager maskProjectileManager)
    {
        _maskInfo = maskInfo;
        _maskProjectileManager = maskProjectileManager;

        if (maskInfo.projectileLifetime > 0)
        {
            Invoke(nameof(DestroySelf), _maskInfo.projectileLifetime);
        }
    }

    public void SetTarget(Transform targetTransform)
    {
        _targetTransform = targetTransform;
        _lastTargetPosition = _targetTransform.position;
    }

    private void FixedUpdate()
    {
        if (!_maskInfo || !_maskProjectileManager) return;
        if (_isBeingDestroyed) return;

        if (_maskInfo.spawnType == MaskInfo.ESpawnType.target)
        {
            if (_targetTransform)
            {
                _lastTargetPosition = Utils.Vector3XY(_targetTransform.position, transform.position);
            }

            Vector3 direction = (_lastTargetPosition - transform.position).normalized;
            Vector3 startPosition = transform.position;
            Vector3 endPosition = startPosition + (direction * (_maskInfo.projectileSpeed * Time.fixedDeltaTime));
            rigidBody.MovePosition(endPosition);

            if (Vector3.Distance(transform.position, _lastTargetPosition) < 0.1f || Utils.IsPointBetween(startPosition, endPosition, _lastTargetPosition))
            {
                BounceOrDestroy();
            }
        }
        else if (_maskInfo.spawnType != MaskInfo.ESpawnType.orbital)
        {
            var previousPosition = transform.position;
            var nextPosition = transform.position +
                               (transform.forward * (_maskInfo.projectileSpeed * Time.fixedDeltaTime));
            rigidBody.MovePosition(nextPosition);
            _distanceTraveled += Vector3.Distance(previousPosition, nextPosition);

            if (_distanceTraveled >= _maskInfo.range)
            {
                DestroySelf();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_maskInfo) return;
        if (!other.CompareTag("Enemy")) return;

        var enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth != null && !_hits.Contains(other.gameObject))
        {
            _hits.Add(other.gameObject);

            var damage = PlayerStats.Instance.ScaleDamage(_maskProjectileManager.Damage);

            if (damage > 0)
            {
                enemyHealth.TakeDamage(
                    damage,
                    PlayerStats.Instance.IsCriticalHit()
                );
            }

            SpawnHitEffect(other.transform);
        }

        if (_maskProjectileManager.Bounces > 0)
        {
            BounceOrDestroy();
        }
        else if (!_maskInfo.projectilePiercing && _maskInfo.spawnType != MaskInfo.ESpawnType.orbital)
        {
            DestroySelf();
        }
    }

    private void BounceOrDestroy()
    {
        if (_targetTransform)
        {
            _bouncedTargets.Add(_targetTransform);
        }

        if (_maskProjectileManager.Bounces > 0 && _bounceCount < _maskProjectileManager.Bounces)
        {
            var nextEnemy = Utils
                .FindEnemiesInRange(transform.position, _maskInfo.projectileBounceRange)
                .OrderBy(_ => Random.value)
                .FirstOrDefault(e => !_bouncedTargets.Contains(e));

            if (nextEnemy)
            {
                SetTarget(nextEnemy);
                _bounceCount++;
            }
            else
            {
                DestroySelf();
            }
        }
        else
        {
            DestroySelf();
        }
    }

    private void SpawnHitEffect(Transform hitTransform)
    {
        if (_maskInfo.splashPrefab != null)
        {
            var spawnPosition = Utils.Vector3XY(hitTransform ? hitTransform.position : transform.position, _maskInfo.splashPrefab.transform.position);
            var splashObject = Instantiate(_maskInfo.splashPrefab, spawnPosition, Quaternion.identity);
            splashObject.GetComponent<Splash>().SetUp(_maskProjectileManager);
        }
    }

    private void DestroySelf()
    {
        _isBeingDestroyed = true;

        if (model)
        {
            model.SetActive(false);
        }

        Destroy(gameObject, _maskInfo.projectileDestroyDelay);
    }
}
