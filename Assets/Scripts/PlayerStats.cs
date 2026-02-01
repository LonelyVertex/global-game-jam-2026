using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }
    public float hitpoints = 100f;
    public int maxHitpoints = 100;
    public float xp = 0;
    public int currentLevel = 1;

    [SerializeField] public float hitpointsRegen = 1;
    [SerializeField] public float movementSpeed = 5f;
    [Range(0f, 1f)]
    [SerializeField] public float armor = 0f;
    [Range(0f, 11f)]
    [SerializeField] public float evasion = 0f;
    [SerializeField] public float attackSpeed = 1f;
    [SerializeField] public float difficultyScale = 100f;
    [SerializeField] public float difficultyShape = 1.6f;

    public readonly List<MaskInfo> _equippedMasks = new();
    public readonly Dictionary<MaskInfo, MaskProjectileManager> _maskManagers = new();

    public float movementSpeedBonus = 0f;
    public float regenerationBonus = 0f;
    public float hitpointsBonus = 0f;
    public float damageBonus = 0f;
    public float damageScaler = 1f;

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

    void Start()
    {
        hitpoints = maxHitpoints;
        xp = 0;
        currentLevel = 1;
    }

    void Update()
    {
        // Regenerate hitpoints over time
        if (hitpoints < TotalMaxHitpoints() && !IsDead())
        {
            Heal(Mathf.Clamp(hitpointsRegen * Time.deltaTime, 0, TotalMaxHitpoints()));
        }
    }

    public float TotalMaxHitpoints()
    {
        return maxHitpoints + hitpointsBonus;
    }

    public float MovementSpeed
    {
        get => movementSpeed * (1 + GetEffectiveMovementSpeedup());
        private set => movementSpeed = value;
    }

    public int ScaleDamage(int originalDamage)
    {
        return Mathf.FloorToInt(originalDamage*GetDamangeScaler());
    }

    public float ScaleCooldown(float originalCooldown)
    {
        return originalCooldown / attackSpeed;
    }

    public float ScaleOrbitalSpeed(float originalSpeed)
    {
        return originalSpeed * attackSpeed;
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
        if (IsDead())
        {
            return;
        }
        //apply evasion
        if (IsEvaded())
        {
            Debug.Log("Player evaded the attack!");
            return;
        }
        //apply armor
        float effectiveDamage = damage * (1 - GetEffectiveArmor());

        if (effectiveDamage > 0)
        {
            hitpoints = Mathf.Clamp(hitpoints - effectiveDamage, 0, TotalMaxHitpoints());
            Debug.Log($"Player took {effectiveDamage} damage. Remaining HP: {hitpoints}");
            if (hitpoints <= 0)
            {
                Die();
            }
        }

    }

    public bool HasMask(MaskInfo maskInfo)
    {
        return _maskManagers.ContainsKey(maskInfo);
    }

    public void EquipMask(MaskInfo maskInfo)
    {
        _equippedMasks.Add(maskInfo);
    }

    public void AddMaskManager(MaskInfo maskInfo, MaskProjectileManager manager)
    {
        _maskManagers[maskInfo] = manager;
    }

    public MaskProjectileManager GetMaskManager(MaskInfo maskInfo)
    {
        return _maskManagers[maskInfo];
    }

    private void Die()
    {
        GameManager.Instance.ShowDeathScreen();
    }

    public void Heal(float amount)
    {
        hitpoints = Mathf.Clamp(hitpoints + amount, 0, TotalMaxHitpoints());
    }

    public bool IsDead()
    {
        return hitpoints <= 0;
    }

    public void AddXp(float amount)
    {
        xp += amount;
        var levelBasedOnCurrentXp = LevelFromTotalXp(Mathf.FloorToInt(xp));
        if (currentLevel < levelBasedOnCurrentXp)
        {
            currentLevel = levelBasedOnCurrentXp;
            GameManager.Instance.ShowSkillSelection();
            Debug.Log($"Player leveled up to level {levelBasedOnCurrentXp}!");
        }

    }

    public int TotalXpForLevel(int level)
    {
        level = Mathf.Max(level, 1);
        float x = level - 1;
        return Mathf.FloorToInt(difficultyScale * Mathf.Pow(x, difficultyShape));
    }

    // XP required to go FROM level -> level+1
    public int XpToNextLevel(int level)
    {
        int cur = TotalXpForLevel(level);
        int next = TotalXpForLevel(level + 1);
        return next - cur;
    }

    // Given total XP, compute current level
    public int LevelFromTotalXp(int totalXp)
    {
        totalXp = Mathf.Max(totalXp, 0);
        // Invert: totalXp = A*(L-1)^P  =>  L = 1 + (totalXp/A)^(1/P)
        float x = Mathf.Pow(totalXp / difficultyScale, 1f / difficultyShape);
        return Mathf.FloorToInt(x) + 1;
    }

    public float GetEffectiveArmor()
    {
        return ScalingFunction(1, armor);
    }

    public float GetEffectiveMovementSpeedup()
    {
        return ScalingFunction(1, movementSpeedBonus);
    }

    public bool IsEvaded()
    {
        return Random.value < ScalingFunction(10f, evasion);
    }

    public float GetRegenerationValue()
    {
        return hitpointsRegen+ScalingFunction(1, regenerationBonus);
    }

    public float GetDamangeScaler()
    {
        return damageScaler+ScalingFunction(1, damageBonus);
    }

    public float ScalingFunction(float K, float value)
    {
        return value / (value + K);
    }

    public void TakeSkill(SkillInfo skillInfo)
    {
        switch (skillInfo.type)
        {
            case SkillInfo.SkillType.movementSpeed:
                movementSpeedBonus += skillInfo.value;
                break;
            case SkillInfo.SkillType.armor:
                armor += skillInfo.value;
                break;
            case SkillInfo.SkillType.evasion:
                evasion += skillInfo.value;
                break;
            case SkillInfo.SkillType.regeneration:
                regenerationBonus += skillInfo.value;
                break;
            case SkillInfo.SkillType.attack_speed:
                attackSpeed += skillInfo.value;
                break;
            case SkillInfo.SkillType.hitpoints:
                hitpointsBonus += Mathf.FloorToInt(skillInfo.value);
                break;
            case SkillInfo.SkillType.damage:
                damageBonus += skillInfo.value;
                break;
            default:
                Debug.LogWarning("Unknown skill type.");
                break;
        }
    }

}
