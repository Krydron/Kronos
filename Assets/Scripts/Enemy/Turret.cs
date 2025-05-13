using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Turret Components")]
    public Transform turretHead;  // The rotating part
    public Transform firePoint;   // Where bullets spawn
    public GameObject projectilePrefab;

    [Header("Turret Settings")]
    public float rotationSpeed = 5f;
    public float fireRate = 1f;
    public float projectileSpeed = 20f;

    private Transform target;
    private float fireCooldown = 0f;

    void Update()
    {
        if (target != null)
        {
            RotateTowardsTarget();
            ShootAtTarget();
        }

        fireCooldown -= Time.deltaTime;
    }

    void RotateTowardsTarget()
    {
        Vector3 direction = (target.position - turretHead.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        turretHead.rotation = Quaternion.Slerp(turretHead.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    void ShootAtTarget()
    {
        if (fireCooldown <= 0f)
        {
            fireCooldown = fireRate;
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = firePoint.forward * projectileSpeed;
            }
            Destroy(bullet, 3f);
        }
    }

    // Called by Trespass Zone
    public void SetTarget(Transform player)
    {
        target = player;
    }

    public void ClearTarget()
    {
        target = null;
    }
}
