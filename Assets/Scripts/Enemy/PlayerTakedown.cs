using UnityEngine;

public class PlayerTakedown : MonoBehaviour
{
    [SerializeField] private float takedownRange = 2f;
    private PlayerMovement playerMovement;
    private Inventory inventory;
    public bool takedownPerformedThisFrame = false; // Flag to communicate with Shooting

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        inventory = GetComponent<Inventory>();
    }

    // Call this from Shooting.OnAttack() when melee weapon is equipped and player is sneaking
    public bool TryTakedown()
    {
        takedownPerformedThisFrame = false; // reset flag each try

        if (!playerMovement.IsSneaking())
            return false;

        // Check if melee weapon is equipped
        Weapon currentWeapon = inventory.GetWeapon();
        if (currentWeapon == null || currentWeapon.name != "Melee")
            return false;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, takedownRange);

        foreach (var hitCollider in hitColliders)
        {
            EnemyBase enemy = hitCollider.GetComponent<EnemyBase>();
            if (enemy != null && hitCollider.CompareTag("Enemy") && IsBehindEnemy(enemy))
            {
                PerformTakedown(enemy);
                takedownPerformedThisFrame = true;
                return true;
            }
        }

        return false;
    }

    bool IsBehindEnemy(EnemyBase enemy)
    {
        Vector3 directionToPlayer = (transform.position - enemy.transform.position).normalized;
        Vector3 enemyForward = enemy.transform.forward;
        float dotProduct = Vector3.Dot(enemyForward, directionToPlayer);
        return dotProduct < 0;
    }

    void PerformTakedown(EnemyBase enemy)
    {
        Debug.Log("Takedown successful!");
        enemy.TakeDamage(enemy.maxHealth);
    }
}
