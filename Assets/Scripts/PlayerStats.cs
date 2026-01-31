using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }
    public float hitpoints = 100f;
    public int maxHitpoints = 100;
    public float xp = 0;
    public int currentLevel = 1;

    [SerializeField] private float hitpointsRegen = 1;
    [SerializeField] private float movementSpeed = 5f;
    [Range(0f, 1f)]
    [SerializeField] private float armor = 0f;
    [Range(0f, 11f)]
    [SerializeField] private float evasion = 0f;
    [SerializeField] private float attackSpeed = 1f;

    public readonly List<MaskInfo> _equippedMasks = new();

    private float movementSpeedBonus = 0f;
    private float regenerationBonus = 0f;

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
        if (hitpoints < maxHitpoints && !IsDead())
        {
            Heal(Mathf.Clamp(hitpointsRegen * Time.deltaTime, 0, maxHitpoints));
        }
    }

    public float MovementSpeed
    {
        get => movementSpeed * (1 + GetEffectiveMovementSpeedup());
        private set => movementSpeed = value;
    }

    public int ScaleDamage(int originalDamage)
    {
        return originalDamage;
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
            hitpoints = Mathf.Clamp(hitpoints - effectiveDamage, 0, maxHitpoints);
            Debug.Log($"Player took {effectiveDamage} damage. Remaining HP: {hitpoints}");
            if (hitpoints <= 0)
            {
                Die();
            }
        }

    }

    public void EquipMask(MaskInfo maskInfo)
    {
        _equippedMasks.Add(maskInfo);
    }

    private void Die()
    {
        Debug.Log("Player has died.");
    }

    public void Heal(float amount)
    {
        hitpoints = Mathf.Clamp(hitpoints + amount, 0, maxHitpoints);
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

    public int TotalXpForLevel(int level, float A = 100f, float P = 2.0f)
    {
        level = Mathf.Max(level, 1);
        float x = level - 1;
        return Mathf.FloorToInt(A * Mathf.Pow(x, P));
    }

    // XP required to go FROM level -> level+1
    public int XpToNextLevel(int level, float A = 100f, float P = 2.0f)
    {
        int cur = TotalXpForLevel(level, A, P);
        int next = TotalXpForLevel(level + 1, A, P);
        return next - cur;
    }

    // Given total XP, compute current level
    public int LevelFromTotalXp(int totalXp, float A = 100f, float P = 2.0f)
    {
        totalXp = Mathf.Max(totalXp, 0);
        // Invert: totalXp = A*(L-1)^P  =>  L = 1 + (totalXp/A)^(1/P)
        float x = Mathf.Pow(totalXp / A, 1f / P);
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
            default:
                Debug.LogWarning("Unknown skill type.");
                break;
        }
    }

}
