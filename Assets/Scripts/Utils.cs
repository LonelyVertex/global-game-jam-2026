using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utils
{
    public static IEnumerable<Transform> FindEnemiesInRange(Vector3 position, float range)
    {
        return Physics.OverlapSphere(position, range, LayerMask.GetMask("Enemy"))
            .Select(collider => collider.transform)
            .Distinct()
            .Where(t => IsInLoS(position, t.position));
    }

    public static IEnumerable<Transform> FindEnemiesInRangeSorted(Vector3 position, float range)
    {
        return FindEnemiesInRange(position, range)
            .OrderBy(e => Vector3.Distance(position, e.position));

    }

    private static bool IsInLoS(Vector3 targetPosition, Vector3 sourcePosition)
    {
        var direction = targetPosition - sourcePosition;
        float distance = Vector3.Distance(targetPosition, sourcePosition);
        int layerMask = LayerMask.GetMask("Obstacles");

        return !Physics.Raycast(sourcePosition, direction, distance, layerMask);
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

    public static bool IsPointBetween(Vector3 start, Vector3 end, Vector3 target)
    {
        float d1 = Vector3.Distance(start, target);
        float d2 = Vector3.Distance(target, end);
        float lineLen = Vector3.Distance(start, end);
        return Mathf.Approximately(d1 + d2, lineLen);
    }
}
