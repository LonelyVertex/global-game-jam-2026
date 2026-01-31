using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

public class RuntimeNavMesh : MonoBehaviour
{

    [SerializeField] private NavMeshSurface surface;

    public IEnumerator BuildNavMesh()
    {
        // If the plane is spawned/generated at runtime, wait a frame so meshes/colliders exist.
        yield return null;

        surface.BuildNavMesh();
    }
}
