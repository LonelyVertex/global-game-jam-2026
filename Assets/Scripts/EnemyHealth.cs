using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private Renderer materialRenderer;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material damageMaterial;
    [SerializeField] private float damageEffectTime;
    [SerializeField] private float health = 100f;

    private float _resetMaterialTime;

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

    private void Update()
    {
        if (_resetMaterialTime >= 0 && Time.time >= _resetMaterialTime)
        {
            materialRenderer.material = defaultMaterial;
            _resetMaterialTime = 0;
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died.");
        Destroy(gameObject);
    }
}
