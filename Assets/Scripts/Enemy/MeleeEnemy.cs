using System.Collections;
using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    public int damage = 20;
    public float attackCooldown = 1.5f;
    private bool canAttack = true;

    [Header("Melee Attack")]
    [SerializeField] float attackDistance = 2f;
    public GameObject meleeHitbox; // Assign in Inspector

    private MeleeHitbox hitboxScript;

    private void Awake()
    {
        if (meleeHitbox != null)
        {
            hitboxScript = meleeHitbox.GetComponent<MeleeHitbox>();
            meleeHitbox.SetActive(false);
        }
    }

    protected override void Attack()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > attackDistance)
        {
            currentState = EnemyState.Chasing;
            return;
        }

        if (canAttack)
        {
            StartCoroutine(PerformMeleeAttack());
        }
    }

    private IEnumerator PerformMeleeAttack()
    {
        canAttack = false;
        Debug.Log("Melee enemy swings!");

        // Enable hitbox and initialize damage
        if (meleeHitbox != null)
        {
            hitboxScript.Initialize(this, damage);
            meleeHitbox.SetActive(true);
        }

        // Attack swing duration - adjust as needed
        yield return new WaitForSeconds(0.3f);

        // Disable hitbox after swing
        if (meleeHitbox != null)
        {
            meleeHitbox.SetActive(false);
        }

        yield return new WaitForSeconds(attackCooldown - 0.3f);
        canAttack = true;
    }
}
