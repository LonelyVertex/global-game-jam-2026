using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MaskList", menuName = "Scriptable Objects/MaskList")]
public class MaskList : ScriptableObject
{
    public List<MaskInfo> masks;
}
