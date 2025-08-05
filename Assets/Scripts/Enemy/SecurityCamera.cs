using System.Collections;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    [Range(1f, 179f)] public float fieldOfView = 45f;  // Horizontal fan angle
    public float viewDistance = 10f;
    [Range(0f, 90f)] public float downwardTiltAngle = 60f;

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
        if (canRotate && !isWaiting) RotateCamera();

        if (PlayerInFanView())
        {
            detectionTimer += Time.deltaTime;
            fovCone?.SetAlert(true);

            if (detectionTimer >= detectionTime)
            {
                if (canTriggerLockdown) TriggerLockdown();
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

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(0, targetRotationY, 0),
            step
        );

        if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetRotationY)) < 1f)
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

    private bool PlayerInFanView()
    {
        if (player == null || fovCone == null) return false;

        Vector3 origin = fovCone.transform.position;
        int segments = fovCone.resolution;

        // Horizontal FOV raycasts
        for (int i = 0; i <= segments; i++)
        {
            float angle = -fieldOfView / 2f + (fieldOfView * i / segments);
            Quaternion rot = Quaternion.Euler(0f, angle, 0f);
            Quaternion tilt = Quaternion.Euler(downwardTiltAngle, 0f, 0f);
            Vector3 dir = fovCone.transform.rotation * (tilt * (rot * Vector3.forward));

            if (Physics.Raycast(origin, dir, out RaycastHit hit, viewDistance))
            {
                if (hit.transform.CompareTag("Player"))
                    return true;
            }
        }

        return false;
    }




    private void TriggerLockdown()
    {
        Debug.Log("Lockdown triggered by camera!");
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
                enemyScript.ReceiveCameraAlert(lastKnownPosition);
        }
    }

    public void SetRotationEnabled(bool enabled) => canRotate = enabled;
}
