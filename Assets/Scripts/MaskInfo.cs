using UnityEngine;

[CreateAssetMenu(fileName = "MaskInfo", menuName = "Scriptable Objects/MaskInfo")]
public class MaskInfo : ScriptableObject
{
    public enum ESpawnType
    {
        target = 0,
        direction = 1,
        frontal = 2,
        orbital = 3,
        melee = 4,
    }

    public string maskName;
    public string description;

    public int damage;
    public float range;
    public float cooldown;
    public ESpawnType spawnType;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpawnRadius;
    public float projectileSpeed;
    public int projectileCount;
    public bool projectilePiercing;
    [Tooltip("Bounce only works with target spawn type")]
    public int projectileBounce;
    public float projectileBounceRange;
    public float projectileDamageDelay;
    public float projectileDestroyDelay;
    public float projectileLifetime;

    [Header("Frontal")]
    public float frontalSpreadAngle;

    [Header("Orbital Settings")]
    public float orbitalSpeed;

    [Header("Splash Settings")]
    public GameObject splashPrefab;
    public int splashDamage;
    public float splashRadius;
    public float splashDamageDelay;
    public float splashLifetime;

    [Header("Mask Visuals")]
    public GameObject screenMaskPrefab;
    public GameObject playerMaskPrefab;

    [Header("Sound Effects")]
    public AudioClip shootSound;
}
