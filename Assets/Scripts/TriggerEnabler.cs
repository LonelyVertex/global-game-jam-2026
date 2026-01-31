using UnityEngine;

public class TriggerEnabler : MonoBehaviour
{
    [SerializeField] private Collider targetTrigger;
    [SerializeField] private float delay;

    void Start()
    {
        Invoke(nameof(EnableTrigger), delay);
    }

    private void EnableTrigger()
    {
        targetTrigger.enabled = true;
    }
}
