using System.Collections;
using UnityEngine;

public class SecurityCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public float fieldOfView = 45f;
    public float viewDistance = 10f;
    public float detectionTime = 2f;
    public float rotationSpeed = 30f; // Speed of rotation
    public float rotationAngle = 45f; // How far the camera turns left and right
    public float rotationDelay = 2f; // Pause before changing direction

    [Header("Vision Cone")]
    public MeshFilter visionMeshFilter;
    public MeshRenderer visionRenderer;
    public int coneSegments = 20; // Number of segments for the cone

    [Header("Lockdown Settings")]
    public bool canTriggerLockdown = false; // Whether this camera can trigger a lockdown

    private Mesh visionMesh;
    private float detectionTimer = 0f;
    private Transform player;

    private bool rotatingRight = true;
    private bool isWaiting = false;
    private float startRotationY;
    private bool canRotate = true; // New flag to control rotation

    private Color defaultVisionColor = new Color(1f, 1f, 0f, 0.2f); // Yellow (transparent)
    private Color alertVisionColor = new Color(1f, 0f, 0f, 0.2f); // Red (transparent)

    private void Start()
    {
        visionMesh = new Mesh();
        visionMeshFilter.mesh = visionMesh;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        startRotationY = transform.eulerAngles.y;
        GenerateVisionCone();
    }

    private void Update()
    {
        if (canRotate && !isWaiting) // Only rotate if canRotate is true
        {
            RotateCamera();
        }

        if (PlayerInView())
        {
            detectionTimer += Time.deltaTime;
            if (detectionTimer >= detectionTime)
            {
                if (canTriggerLockdown)
                {
                    TriggerLockdown();
                }
                AlertEnemies();
            }

            // Change the vision cone color to red when player is detected
            SetVisionConeColor(alertVisionColor);
        }
        else
        {
            detectionTimer = 0;

            // Change the vision cone color to yellow when player is not detected
            SetVisionConeColor(defaultVisionColor);
        }
    }

    private void RotateCamera()
    {
        // Determine the target rotation angle based on direction
        float targetRotationY = startRotationY + (rotatingRight ? rotationAngle : -rotationAngle);
        float step = rotationSpeed * Time.deltaTime;

        // Normalize the angles to avoid issues with 360 degree wrapping
        float currentRotationY = transform.eulerAngles.y;
        if (currentRotationY > 180f)
            currentRotationY -= 360f; // Normalize between -180 to 180 degrees

        float targetRotation = targetRotationY;
        if (targetRotationY > 180f)
            targetRotationY -= 360f; // Normalize target to -180 to 180 range

        // Calculate the angle difference
        float angleDifference = Mathf.DeltaAngle(currentRotationY, targetRotation);

        // Rotate towards the target angle
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, currentRotationY + angleDifference, 0), step);

        // If the camera is close to the target angle, initiate the delay to switch directions
        if (Mathf.Abs(angleDifference) < 1f)
        {
            StartCoroutine(PauseBeforeSwitching());
        }
    }

    private IEnumerator PauseBeforeSwitching()
    {
        isWaiting = true;
        yield return new WaitForSeconds(rotationDelay);
        rotatingRight = !rotatingRight; // Switch direction
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

                // Ensure the hit object is the player & not a wall
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
        // Here, trigger the lockdown across all cameras or related systems
        //DoorManager.lockdownTriggered = true; // Trigger lockdown globally
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


    private void GenerateVisionCone()
    {
        Vector3[] vertices = new Vector3[coneSegments + 2];
        int[] triangles = new int[coneSegments * 3];

        vertices[0] = Vector3.zero;

        float angleStep = fieldOfView / coneSegments;
        float startAngle = -fieldOfView / 2;

        for (int i = 0; i <= coneSegments; i++)
        {
            float angle = startAngle + i * angleStep;
            float radian = Mathf.Deg2Rad * angle;
            vertices[i + 1] = new Vector3(Mathf.Sin(radian), -0.2f, Mathf.Cos(radian)) * viewDistance;
        }

        for (int i = 0; i < coneSegments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        visionMesh.vertices = vertices;
        visionMesh.triangles = triangles;
        visionMesh.RecalculateNormals();

        if (visionRenderer != null)
        {
            // Set the vision cone color to yellow by default
            visionRenderer.material.color = defaultVisionColor;
        }
    }

    private void SetVisionConeColor(Color color)
    {
        if (visionRenderer != null)
        {
            visionRenderer.material.color = color;
        }
    }

    // Method to enable or disable rotation
    public void SetRotationEnabled(bool enabled)
    {
        canRotate = enabled;
    }
}
