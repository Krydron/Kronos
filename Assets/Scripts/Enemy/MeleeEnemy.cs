/**************************************************************************************************************
* <Name> Class
*
* The header file for the <Name> class.
* 
* This class 
* 
*
* Created by: <Kry> 
* Date: <need to add>
*
***************************************************************************************************************/

using System.Collections;
using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    public int damage = 20;
    public float attackCooldown = 1.5f;
    private bool canAttack = true;
    [SerializeField] float attackDistance;

    [Header("Melee Attack")]
    public GameObject meleeHitbox; // Assign a hitbox in Unity

    protected override void Attack()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > 2f)
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

        //meleeHitbox.SetActive(true); // Activate hitbox for the attack
        //yield return new WaitForSeconds(0.3f); // Attack duration
        //meleeHitbox.SetActive(false);


        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, attackDistance))
        {
            if (hit.transform.CompareTag("Player"))
            {
                Debug.Log("Hit Player");
                hit.rigidbody.gameObject.GetComponent<PlayerHealth>().DecrementHealth(damage);
            }
            Debug.Log("Hit: "+hit.transform.name);
        }
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
