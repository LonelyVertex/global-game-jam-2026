using UnityEngine;

[CreateAssetMenu(fileName = "MaskInfo", menuName = "Scriptable Objects/MaskInfo")]
public class MaskInfo : ScriptableObject
{
    public string maskName;

    public GameObject projectilePrefab;
    public bool projectileTargeted;
    public float projectileSpawnRadius;

    public int damage;
    public float range;
    public float cooldown;

    public int projectileCount;

}
