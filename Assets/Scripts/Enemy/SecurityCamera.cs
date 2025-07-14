using System.Collections;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public float fieldOfView = 45f;
    public float viewDistance = 10f;
    public float detectionTime = 2f;
    public float rotationSpeed = 30f;
    public float rotationAngle = 45f;
    public float rotationDelay = 2f;

    [Header("Spotlight Settings")]
    public Light spotlight;
    public Color idleColor = new Color(1f, 1f, 0f, 1f); // Yellow
    public Color alertColor = new Color(1f, 0f, 0f, 1f); // Red

    [Header("Lockdown Settings")]
    public bool canTriggerLockdown = false;

    private float detectionTimer = 0f;
    private Transform player;

    private bool rotatingRight = true;
    private bool isWaiting = false;
    private float startRotationY;
    private bool canRotate = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        startRotationY = transform.eulerAngles.y;

        if (spotlight != null)
        {
            // Initialize spotlight to match FOV and distance
            spotlight.spotAngle = fieldOfView;
            spotlight.range = viewDistance;
            spotlight.color = idleColor;
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

            if (spotlight != null)
                spotlight.color = alertColor;

            if (detectionTimer >= detectionTime)
            {
                if (canTriggerLockdown)
                {
                    TriggerLockdown();
                }
                AlertEnemies();
            }
        }
        else
        {
            detectionTimer = 0;

            if (spotlight != null)
                spotlight.color = idleColor;
        }

        UpdateSpotlightSettings();
    }

    private void UpdateSpotlightSettings()
    {
        if (spotlight != null)
        {
            spotlight.spotAngle = fieldOfView;
            spotlight.range = viewDistance;
        }
    }

    private void RotateCamera()
    {
        float targetRotationY = startRotationY + (rotatingRight ? rotationAngle : -rotationAngle);
        float step = rotationSpeed * Time.deltaTime;

        float currentRotationY = transform.eulerAngles.y;
        if (currentRotationY > 180f)
            currentRotationY -= 360f;

        float target = targetRotationY;
        if (targetRotationY > 180f)
            targetRotationY -= 360f;

        float angleDifference = Mathf.DeltaAngle(currentRotationY, target);

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
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle < fieldOfView / 2)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, viewDistance))
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
        //DoorManager.lockdownTriggered = true;
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
