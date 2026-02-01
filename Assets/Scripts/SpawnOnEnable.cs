using UnityEngine;

public class SpawnOnEnable : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public float duration;

    private GameObject _spawnedObject;

    private void Start()
    {
        _spawnedObject = Instantiate(prefabToSpawn, transform.position, transform.rotation);
        Invoke(nameof(DestroyObject), duration);
    }

    private void DestroyObject()
    {
        if (_spawnedObject == null)
        {
            return;
        }

        Destroy(_spawnedObject);
    }
}
