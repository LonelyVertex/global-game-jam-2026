using UnityEngine;

public class MaskBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.ShowMaskSelection();
            Destroy(gameObject);
        }
    }
}
