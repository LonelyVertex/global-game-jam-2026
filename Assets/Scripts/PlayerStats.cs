using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }
    [SerializeField] private float hitpoints = 100f;
    [SerializeField] private int maxHitpoints = 100;
    [SerializeField] private float hitpointsRegen = 1;
    [SerializeField] private float movementSpeed = 5f;
    [Range(0f, 1f)]
    [SerializeField] private float armor = 0.1f;
    [Range(0f, 11f)]
    [SerializeField] private float evasion = 0.1f;
    [SerializeField] private float attackSpeed = 5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Regenerate hitpoints over time
        if (hitpoints < maxHitpoints)
        {
            Heal(Mathf.Clamp(hitpointsRegen * Time.deltaTime, 0, maxHitpoints));
        }
    }

    public float MovementSpeed
    {
        get => movementSpeed;
        private set => movementSpeed = value;
    }

    public int ScaleDamage(int originalDamage)
    {
        return originalDamage;
    }

    public float ScaleCooldown(float originalCooldown)
    {
        return originalCooldown;
    }

    public bool IsCriticalHit()
    {
        // TODO
        return Random.value < 0.2f;
    }

    public int ScaleProjectileCount(int projectileCount)
    {
        return projectileCount;
    }

    public void TakeDamage(float damage)
    {
        //apply evasion
        if (Random.value < evasion)
        {
            Debug.Log("Player evaded the attack!");
            return; 
        }
        //apply armor
        float effectiveDamage = damage * (1 - armor);

        if (effectiveDamage > 0)
        {
            hitpoints -= effectiveDamage;
            Debug.Log($"Player took {effectiveDamage} damage. Remaining HP: {hitpoints}");
            if (hitpoints <= 0)
            {
                Die();
            }
        }

    }

    private void Die()
    {
        Debug.Log("Player has died.");
    }

    public void Heal(float amount)
    {
        hitpoints = Mathf.Clamp(hitpoints += amount, 0, maxHitpoints);
    }

}
