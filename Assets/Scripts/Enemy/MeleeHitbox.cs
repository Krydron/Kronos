using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    public int damage = 20;
    private MeleeEnemy owner;

    public void Initialize(MeleeEnemy enemy, int dmg)
    {
        owner = enemy;
        damage = dmg;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.DecrementHealth(damage);
                Debug.Log("Player hit by melee!");
            }
        }
    }
}
