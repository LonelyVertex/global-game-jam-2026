using UnityEngine;

public class Splash : MonoBehaviour
{
    [SerializeField] private MaskInfo maskInfo;

    private MaskProjectileManager _maskProjectileManager;

    public void SetUp(MaskProjectileManager maskProjectileManager)
    {
        _maskProjectileManager = maskProjectileManager;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maskInfo.splashRadius);
    }

    private void Start()
    {
        Invoke(nameof(DealDamage), maskInfo.splashDamageDelay);
        Destroy(gameObject, maskInfo.splashLifetime);

        if (maskInfo.splashSound != null)
        {
            SoundManager.Instance.PlaySound(maskInfo.splashSound);
        }
    }

    private void DealDamage()
    {
        var enemies = Utils.FindEnemiesInRange(transform.position, maskInfo.splashRadius);

        foreach (var enemyCollider in enemies)
        {
            var enemyHealth = enemyCollider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(
                    PlayerStats.Instance.ScaleDamage(_maskProjectileManager.SplashDamage),
                    PlayerStats.Instance.IsCriticalHit());
            }
        }
    }
}
