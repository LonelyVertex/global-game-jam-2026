using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillsList", menuName = "Scriptable Objects/SkillsList")]
public class SkillsList : ScriptableObject
{
    public List<SkillInfo> skills;
}
