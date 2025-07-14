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
using FMODUnity;  

public class RangedEnemy : EnemyBase
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootCooldown = 1.5f;
    private bool canShoot = true;

    [Header("Shooting Audio")]
    public EventReference shootSoundEvent;
    public EventReference reloadSoundEvent;

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

        firePoint.LookAt(new Vector3(player.transform.position.x, player.transform.position.y + 0.5f, player.transform.position.z));

        // Play shooting sound
        RuntimeManager.PlayOneShot(shootSoundEvent, transform.position);

        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Play reload sound during cooldown
        RuntimeManager.PlayOneShot(reloadSoundEvent, transform.position);

        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }
}
