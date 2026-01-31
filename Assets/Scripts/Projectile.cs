using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private MaskInfo maskInfo;
    [SerializeField] private Rigidbody rigidBody;

    private Transform _targetTransform;
    private Vector3 _lastTargetPosition;

    private float _distanceTraveled;

    private int _bounceCount;
    private readonly List<Transform> _bouncedTargets = new();

    private bool _isBeingDestroyed;

    public void SetTarget(Transform targetTransform)
    {
        _targetTransform = targetTransform;
        _lastTargetPosition = _targetTransform.position;
    }

    private void FixedUpdate()
    {
        if (_isBeingDestroyed) return;

        if (maskInfo.spawnType == MaskInfo.ESpawnType.target)
        {
            if (_targetTransform)
            {
                _lastTargetPosition = _targetTransform.position;
            }

            Vector3 direction = (_lastTargetPosition - transform.position).normalized;
            rigidBody.MovePosition(transform.position + (direction * (maskInfo.projectileSpeed * Time.fixedDeltaTime)));

            // destroy the projectile if it reaches the last known position of the target, and it was destroyed while it was flying
            if (Vector3.Distance(transform.position, _lastTargetPosition) < 0.1f)
            {
                if (_targetTransform)
                {
                    _bouncedTargets.Add(_targetTransform);
                }

                if (maskInfo.projectileBounce > 0 && _bounceCount < maskInfo.projectileBounce)
                {
                    var nextEnemy = Utils
                        .FindEnemiesInRangeSorted(transform.position, maskInfo.projectileBounceRange)
                        .FirstOrDefault(e => !_bouncedTargets.Contains(e));

                    if (nextEnemy != null)
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
        }
        else
        {
            var previousPosition = transform.position;
            var nextPosition = transform.position +
                               (transform.forward * (maskInfo.projectileSpeed * Time.fixedDeltaTime));
            rigidBody.MovePosition(nextPosition);
            _distanceTraveled += Vector3.Distance(previousPosition, nextPosition);

            if (_distanceTraveled >= maskInfo.range)
            {
                DestroySelf();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        var enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(
                PlayerStats.Instance.ScaleDamage(maskInfo.damage),
                PlayerStats.Instance.IsCriticalHit()
            );

            SpawnHitEffect();
        }

        if (!maskInfo.projectilePiercing && maskInfo.spawnType != MaskInfo.ESpawnType.orbital)
        {
            Destroy(gameObject);
        }
    }

    private void SpawnHitEffect()
    {
        if (maskInfo.splashPrefab != null)
        {
            Instantiate(maskInfo.splashPrefab, transform.position, Quaternion.identity);
        }
    }

    private void DestroySelf()
    {
        _isBeingDestroyed = true;
        Destroy(gameObject, maskInfo.projectileDestroyDelay);
    }
}
