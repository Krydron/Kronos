using UnityEngine;

public class PlayerTakedown : MonoBehaviour
{
    [SerializeField] private float takedownRange = 2f; // Distance required for takedown
    private PlayerMovement playerMovement; // Reference to check if the player is sneaking

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // Check if the V key is pressed for the melee attack while sneaking
        if (Input.GetKeyDown(KeyCode.V) && playerMovement.IsSneaking()) // Replace with your melee button
        {
            TryTakedown();
        }
    }

    void TryTakedown()
    {
        // Find nearby enemies using OverlapSphere to detect enemies within the takedown range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, takedownRange);

        foreach (var hitCollider in hitColliders)
        {
            EnemyBase enemy = hitCollider.GetComponent<EnemyBase>();
            if (enemy != null && hitCollider.CompareTag("Enemy") && IsBehindEnemy(enemy)) // Check if the tag is "Enemy" and player is behind the enemy
            {
                PerformTakedown(enemy);
                return; // Once a takedown is performed, exit the loop
            }
        }
    }

    bool IsBehindEnemy(EnemyBase enemy)
    {
        // Get the direction from the enemy to the player
        Vector3 directionToPlayer = (transform.position - enemy.transform.position).normalized;

        // Get the enemy's forward direction (the direction they're facing)
        Vector3 enemyForward = enemy.transform.forward;

        // Debug logs to visualize direction vectors
        Debug.DrawRay(enemy.transform.position, enemyForward * 2, Color.red);   // Enemy's forward direction (red)
        Debug.DrawRay(enemy.transform.position, directionToPlayer * 2, Color.green); // Direction to the player (green)

        // Check the dot product between the enemy's forward vector and the direction to the player
        float dotProduct = Vector3.Dot(enemyForward, directionToPlayer);

        // If the dot product is negative, the player is behind the enemy
        bool isBehind = dotProduct < 0;

        if (isBehind)
        {
            Debug.Log("Player is behind the enemy!");
        }
        else
        {
            Debug.Log("Player is not behind the enemy.");
        }

        return isBehind;
    }





    void PerformTakedown(EnemyBase enemy)
    {
        // Ensure we directly kill the enemy by dealing its maximum health as damage
        Debug.Log("Takedown successful!");
        enemy.TakeDamage(enemy.GetComponent<EnemyBase>().maxHealth); // Instantly kill the enemy by dealing max health damage
    }
}
