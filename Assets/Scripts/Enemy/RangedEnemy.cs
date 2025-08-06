using System.Collections;
using UnityEngine;
using FMODUnity;

public class RangedEnemy : EnemyBase
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootCooldown = 1.5f;
    private bool canShoot = true;

    [Header("Shooting Audio Emitters")]
    public StudioEventEmitter shootEmitter;
    public StudioEventEmitter reloadEmitter;

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

        // Aim at player
        firePoint.LookAt(new Vector3(player.transform.position.x, player.transform.position.y + 0.5f, player.transform.position.z));

        // Play shooting sound from emitter
        if (shootEmitter != null) shootEmitter.Play();

        // Spawn bullet
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Play reload sound from emitter
        if (reloadEmitter != null) reloadEmitter.Play();

        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }
}
