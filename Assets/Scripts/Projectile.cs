using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private bool piercing;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private GameObject onHitEffectPrefab;

    public int Damage { get; set; }

    private bool _targeted;
    private Transform _targetTransform;
    private Vector3 _lastTargetPosition;

    public void SetTarget(Transform targetTransform)
    {
        _targetTransform = targetTransform;
        _lastTargetPosition = _targetTransform.position;
        _targeted = true;
    }

    private void FixedUpdate()
    {
        if (_targeted)
        {
            if (_targetTransform)
            {
                _lastTargetPosition = _targetTransform.position;
            }

            Vector3 direction = (_lastTargetPosition - transform.position).normalized;
            rigidBody.MovePosition(transform.position + (direction * (speed * Time.fixedDeltaTime)));

            // destroy the projectile if it reaches the last known position of the target, and it was destroyed while it was flying
            if (!_targetTransform && Vector3.Distance(transform.position, _lastTargetPosition) < 0.1f)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            rigidBody.MovePosition(transform.position + (transform.forward * (speed * Time.fixedDeltaTime)));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        var enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(Damage);

            SpawnHitEffect();
        }

        if (!piercing)
        {
            Destroy(gameObject);
        }
    }

    private void SpawnHitEffect()
    {
        if (onHitEffectPrefab != null)
        {
            Instantiate(onHitEffectPrefab, transform.position, Quaternion.identity);
        }
    }
}
