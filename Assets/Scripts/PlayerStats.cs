using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

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

}
