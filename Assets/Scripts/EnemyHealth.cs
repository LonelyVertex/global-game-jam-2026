using System.Security.Cryptography;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private Renderer materialRenderer;
    [SerializeField] private Material damageMaterial;
    [SerializeField] private float damageEffectTime;
    [SerializeField] public float health = 100f;
    public float likelihoodToSpawnMask = 0.2f;
    public GameObject maskPrefab;

    public float xp = 10f;
    public bool dead = false;

    private float _resetMaterialTime;
    private Material originalMaterial;

    public void TakeDamage(int damage, bool isCritical)
    {
        // Implement damage logic here
        string criticalInfo = isCritical ? " (critical)" : "";
        damage = isCritical ? 2 * damage : damage;
        Debug.Log($"Enemy took {damage}{criticalInfo} damage.");

        WorldSpaceUIController.Instance.DamageNumber(transform.position, damage);

        materialRenderer.material = damageMaterial;
        _resetMaterialTime = Time.time + damageEffectTime;
        health = Mathf.Clamp(health - damage, 0, health);
        if (health <= 0)
        {
            Die();
        }
    }

    private void Awake()
    {
        originalMaterial = materialRenderer.sharedMaterial;
    }

    private void Update()
    {
        if (_resetMaterialTime >= 0 && Time.time >= _resetMaterialTime)
        {
            materialRenderer.material = originalMaterial;
            _resetMaterialTime = 0;
        }
    }

    private void Die()
    {
        if (!dead)
        {
            Debug.Log("Enemy died.");
            
            GameManager.Instance.totalKills += 1;
            GameManager.Instance.activesEnemies.Remove(gameObject);

            PlayerStats.Instance.AddXp(xp);

            if (Random.value < likelihoodToSpawnMask)
            {
                Instantiate(maskPrefab, Utils.Vector3XY(transform.position, maskPrefab.transform.position), Quaternion.identity);
            }
            dead = true;
        }
        Destroy(gameObject);
    }
}
