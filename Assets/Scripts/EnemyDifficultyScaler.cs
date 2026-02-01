using UnityEngine;

public class EnemyDifficultyScaler : MonoBehaviour
{
    public float speedCaler = 0.1f;
    public float healthScaler = 0.2f;
    public float damageScaler = 0.1f;

    public float ScaleSpeedWithLevel(float originalValue,int playerLevel, int enemyStartLevel)
    {
        return ScaleValueWithLevel(originalValue, speedCaler, playerLevel, enemyStartLevel);
    }
    public float ScaleHealthWithLevel(float originalValue, int playerLevel, int enemyStartLevel)
    {
        return ScaleValueWithLevel(originalValue, healthScaler, playerLevel, enemyStartLevel);
    }

    public float ScaleDamageWithLevel(float originalValue, int playerLevel, int enemyStartLevel)
    {
        return ScaleValueWithLevel(originalValue, damageScaler, playerLevel, enemyStartLevel);
    }

    public float ScaleValueWithLevel(float originalValue, float scaler, int playerLevel, int enemyStartLevel)
    {
        return originalValue * (1 + ScalingFunction(1, scaler * (playerLevel - enemyStartLevel)));
    }

    public float ScalingFunction(float K, float value)
    {
        return value / (value + K);
    }

}
