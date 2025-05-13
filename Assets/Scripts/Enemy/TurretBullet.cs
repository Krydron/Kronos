using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    public float damage = 10f;  // Adjustable bullet damage
    public float lifetime = 3f; // Bullet auto-destroys after this time

    private void Start()
    {
        Destroy(gameObject, lifetime); // Destroy bullet after a set time
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.DecrementHealth(damage);
            }

            Destroy(gameObject); // Destroy bullet on impact
        }
    }
}
