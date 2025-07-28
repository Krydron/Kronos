using System.Collections;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    [Range(1f, 179f)] public float fieldOfView = 45f;            // Horizontal FOV (cone half-angle in degrees)
    [Range(1f, 179f)] public float verticalFieldOfView = 45f;    // Vertical FOV (cone half-angle in degrees)
    public float viewDistance = 10f;                             // Cone length
    [Range(0f, 90f)] public float downwardTiltAngle = 60f;       // Tilt cone downward (degrees)

    public float detectionTime = 2f;
    public float rotationSpeed = 30f;
    public float rotationAngle = 45f;
    public float rotationDelay = 2f;

    [Header("FOV Cone Settings")]
    public Material idleFOVMaterial;
    public Material alertFOVMaterial;

    [Header("Lockdown Settings")]
    public bool canTriggerLockdown = false;

    private float detectionTimer = 0f;
    private Transform player;

    private bool rotatingRight = true;
    private bool isWaiting = false;
    private float startRotationY;
    private bool canRotate = true;

    private CameraFOVCone fovCone;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        startRotationY = transform.eulerAngles.y;

        fovCone = GetComponentInChildren<CameraFOVCone>();
        if (fovCone != null)
        {
            fovCone.SetMaterials(idleFOVMaterial, alertFOVMaterial);
            fovCone.SetAlert(false);
        }
    }

    private void Update()
    {
        if (canRotate && !isWaiting)
        {
            RotateCamera();
        }

        if (PlayerInView())
        {
            detectionTimer += Time.deltaTime;
            fovCone?.SetAlert(true);

            if (detectionTimer >= detectionTime)
            {
                if (canTriggerLockdown)
                    TriggerLockdown();

                AlertEnemies();
            }
        }
        else
        {
            detectionTimer = 0f;
            fovCone?.SetAlert(false);
        }
    }

    private void RotateCamera()
    {
        float targetRotationY = startRotationY + (rotatingRight ? rotationAngle : -rotationAngle);
        float step = rotationSpeed * Time.deltaTime;

        float currentRotationY = transform.eulerAngles.y;
        if (currentRotationY > 180f)
            currentRotationY -= 360f;

        if (targetRotationY > 180f)
            targetRotationY -= 360f;

        float angleDifference = Mathf.DeltaAngle(currentRotationY, targetRotationY);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(0, currentRotationY + angleDifference, 0),
            step
        );

        if (Mathf.Abs(angleDifference) < 1f)
        {
            StartCoroutine(PauseBeforeSwitching());
        }
    }

    private IEnumerator PauseBeforeSwitching()
    {
        isWaiting = true;
        yield return new WaitForSeconds(rotationDelay);
        rotatingRight = !rotatingRight;
        isWaiting = false;
    }

    private bool PlayerInView()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Horizontal angle between camera forward and player, ignoring vertical component
        Vector3 forwardHorizontal = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
        Vector3 dirHorizontal = new Vector3(directionToPlayer.x, 0f, directionToPlayer.z).normalized;

        float horizontalAngle = Vector3.Angle(forwardHorizontal, dirHorizontal);

        // Vertical angle between camera forward and player
        float verticalAngle = Vector3.Angle(transform.forward, directionToPlayer) - horizontalAngle;

        if (horizontalAngle < fieldOfView / 2f && Mathf.Abs(verticalAngle) < verticalFieldOfView / 2f)
        {
            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, viewDistance))
            {
                Debug.DrawRay(transform.position, directionToPlayer * viewDistance, Color.red);

                if (hit.transform.CompareTag("Player"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void TriggerLockdown()
    {
        Debug.Log("Lockdown triggered by camera!");
        // Your global lockdown logic here
    }

    private void AlertEnemies()
    {
        if (player == null) return;

        Vector3 lastKnownPosition = player.position;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            EnemyBase enemyScript = enemy.GetComponent<EnemyBase>();
            if (enemyScript != null)
            {
                enemyScript.ReceiveCameraAlert(lastKnownPosition);
            }
        }

        Debug.Log("Security Camera ALERT! Enemies are now searching the area!");
    }

    // Public method to enable/disable rotation
    public void SetRotationEnabled(bool enabled)
    {
        canRotate = enabled;
    }
}
