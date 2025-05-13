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

public class RangedEnemy : EnemyBase
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootCooldown = 1.5f;
    private bool canShoot = true;

    protected override void Attack()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > 8f)
        {
            currentState = EnemyState.Chasing;
            return;
        }

        if (canShoot)
        {
            StartCoroutine(ShootAtPlayer());
        }
    }

    private IEnumerator ShootAtPlayer()
    {
        canShoot = false;
        Debug.Log("Ranged enemy fires!");
        firePoint.LookAt(new Vector3(player.transform.position.x, player.transform.position.y+0.5f, player.transform.position.z));


        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }
}
