using UnityEngine;

public class EnemyWeaponController : MonoBehaviour
{
    private bool inContactWithPlayer = false;
    [SerializeField] private float damageCooldown = 1f;
    private float lastDamageTime = -Mathf.Infinity;
    [SerializeField] public float damage = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inContactWithPlayer)
        {
            DoDamageToPlayer();
        }
    
    }
    
    private void OnTriggerEnter(Collider other)
    {
        //Check for tag "Enemy"
        if (other.CompareTag("Player"))
        {
            inContactWithPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Check for tag "Enemy"
        if (other.CompareTag("Player"))
        {
            inContactWithPlayer = false;
        }
    }

    private void DoDamageToPlayer()
    {
        //Check if enough time has passed since last damage
        if (Time.time >= lastDamageTime + damageCooldown)
        {
            lastDamageTime = Time.time;
            PlayerStats.Instance.TakeDamage((int)damage);
        }
            
    }   
}
