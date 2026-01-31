using UnityEngine;

public class EnemyWeaponController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        //Check for tag "Enemy"
        if (other.CompareTag("Player"))
        {
            Debug.Log("EnemyWeaponController OnTriggerEnter with " + other.name);
        }
    }
}
