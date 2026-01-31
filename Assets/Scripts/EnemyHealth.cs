using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public void TakeDamage(int damage)
    {
        // Implement damage logic here
        Debug.Log($"Enemy took {damage} damage.");
    }
}
