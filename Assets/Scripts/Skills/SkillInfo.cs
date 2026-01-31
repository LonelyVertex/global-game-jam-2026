using UnityEngine;

[CreateAssetMenu(fileName = "SkillInfo", menuName = "Scriptable Objects/SkillInfo")]
public class SkillInfo : ScriptableObject
{
    public enum SkillType
    {
        movementSpeed = 0,
        armor = 1,
        evasion = 2
    }
    public string skillName;
    public string description;
    public float value;
    public SkillType type;

}
