using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utils
{
    public static IEnumerable<Transform> FindEnemiesInRange(Vector3 position, float range)
    {
        return Physics.OverlapSphere(position, range, LayerMask.GetMask("Enemy"))
            .Select(collider => collider.transform);
    }

    public static IEnumerable<Transform> FindEnemiesInRangeSorted(Vector3 position, float range)
    {
        return FindEnemiesInRange(position, range)
            .OrderBy(e => Vector3.Distance(position, e.position));

    }

}
