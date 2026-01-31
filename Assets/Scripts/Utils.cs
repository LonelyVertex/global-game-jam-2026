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

    public static IEnumerable<T> Shuffle<T>(IEnumerable<T> source)
    {
        return source.OrderBy(_ => Random.value);
    }

    public static Vector3 Vector3XY(Vector3 target, Vector3 ySource)
    {
        return new Vector3(target.x, ySource.y, target.z);
    }

    public static Vector3 Vector3XY(Vector3 target, float y)
    {
        return new Vector3(target.x, y, target.z);
    }
}
