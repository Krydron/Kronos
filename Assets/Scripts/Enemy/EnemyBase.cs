using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class EnemyBase : MonoBehaviour
{
    public enum EnemyState { Patrolling, Chasing, Attacking, Searching, Lockdown }
    public EnemyState currentState;

    public Transform head;

    [Header("Patrolling")]
    public Transform[] patrolPoints;                   // Array of patrol waypoints
    private int waypointIndex;                         // Current index in the waypoint array
    private bool isWaiting;                            // Whether the enemy is waiting at a waypoint
    public float patrolWaitTime = 3f;                  // Wait time at each patrol point
    public float patrolSpeed = 3.5f;                   // Speed while patrolling

    [Header("Detection")]
    public GameObject player;                          // Reference to player
    public float sightRange = 15f;                     // Maximum detection range
    public float fieldOfView = 90f;                    // Field of view in degrees
    public float detectionTime = 2f;                   // Time required to fully detect the player
    public float lostTime = 3f;                        // Time before giving up chasing
    [SerializeField] float minDistance = 2f;           // Minimum distance to attack
    public AnimationCurve angleDetectionCurve;         // Curve for modifying detection speed based on angle

    private float detectionTimer;                      // Timer for accumulating detection
    private float lostTimer;                           // Timer after losing sight
    private bool playerInSight;                        // Whether the player is currently visible

    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Keycard Drop")]
    public GameObject keycardPrefab;
    public bool hasKeycard = false;

    [Header("Search Behavior")]
    public float searchWaitTime = 3f;                  // Wait time at each search point
    public float searchRadius = 6f;                    // Radius of search circle
    public int searchPointsCount = 5;                  // Number of points to search
    private List<Vector3> searchPoints = new List<Vector3>(); // Generated search points
    private int searchPointIndex;
    private bool isSearching;
    private Vector3 lastSeenPosition;

    [Header("Spotlight Settings")]
    public Light fovSpotlight;                       // Reference to the spotlight
    public Color spotlightColor = Color.red;         // Color of the FOV light
    public float spotlightIntensity = 40f;           // Brightness
    public float spotlightOffsetForward = 0.5f;      // How far forward from the head
    public float spotlightOffsetUp = 0f;             // Vertical offset
    [Range(0.1f, 1f)]
    public float spotlightInnerAngleFactor = 0.6f;   // % of outer angle



    private NavMeshAgent agent;
    private float chaseSpeed;

    private bool isWaitingAtSearchPoint = false;       // Added: Flag to control waiting during search points to avoid multiple wait coroutines

    public GameObject GetKeycard()
    {
        return hasKeycard ? keycardPrefab : null;
    }

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;

        chaseSpeed = agent.speed;
        currentHealth = maxHealth;

        if (patrolPoints.Length > 0)
        {
            GoToNextWaypoint();
        }

        isWaiting = false;
        detectionTimer = 0f;
        lostTimer = 0f;
        waypointIndex = 0;

        if (fovSpotlight != null)
        {
            fovSpotlight.type = LightType.Spot;
            fovSpotlight.color = spotlightColor;
            fovSpotlight.intensity = spotlightIntensity;
        }


    }

    private void Update()
    {
        
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                break;
            case EnemyState.Chasing:
                Chase();
                break;
            case EnemyState.Attacking:
                Attack();
                break;
            case EnemyState.Searching:
                Search();
                break;
            case EnemyState.Lockdown:
                Lockdown();
                break;
        }

        UpdateSpotlight();

    }

    public bool lockdownActive = false; // Toggle lockdown mode

    public void StartLockdown()
    {
        lockdownActive = true;
        Debug.Log($"{gameObject.name} entered lockdown mode!");
        currentState = EnemyState.Lockdown;
    }

    private void Lockdown()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance > minDistance)
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(player.transform.position);
        }
        else
        {
            currentState = EnemyState.Attacking;
        }
    }

    private void Patrol()
    {
        agent.speed = patrolSpeed;

        // Check for player visibility
        if (CanSeePlayer())
        {
            float angle = Vector3.Angle(head.forward, (player.transform.position - head.position).normalized);
            float curveMultiplier = angleDetectionCurve.Evaluate(angle / (fieldOfView / 2));
            detectionTimer += Time.deltaTime * curveMultiplier;

            if (detectionTimer >= detectionTime)
            {
                currentState = EnemyState.Chasing;
                detectionTimer = 0f;
                return;
            }
        }
        else
        {
            detectionTimer = Mathf.Clamp(detectionTimer - Time.deltaTime, 0, detectionTime);
        }

        // If reached destination and not waiting, wait
        if (!agent.pathPending && agent.remainingDistance <= 0.5f && !isWaiting)
        {
            StartCoroutine(PauseBeforeNextWaypoint());
        }
    }

    private IEnumerator PauseBeforeNextWaypoint()
    {
        isWaiting = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(patrolWaitTime);
        GoToNextWaypoint();
        agent.isStopped = false;
        isWaiting = false;
    }

    public void GoToNextWaypoint()
    {
        if (patrolPoints.Length == 0) return;
        agent.destination = patrolPoints[waypointIndex].position;
        waypointIndex = (waypointIndex + 1) % patrolPoints.Length;
    }

    private void Chase()
    {
        if (player == null) return;

        if (CanSeePlayer())
        {
            playerInSight = true;
            lastSeenPosition = player.transform.position;
            float distance = Vector3.Distance(player.transform.position, transform.position);

            if (distance >= minDistance)
            {
                agent.speed = chaseSpeed;
                agent.destination = player.transform.position;
            }
            else
            {
                currentState = EnemyState.Attacking;
            }

            detectionTimer = 0f;
            lostTimer = 0f;
        }
        else
        {
            playerInSight = false;
            lostTimer += Time.deltaTime;

            if (lostTimer >= 1f)
            {
                currentState = EnemyState.Searching;
                PrepareSearch(lastSeenPosition);
            }
        }
    }

    // Updated Search method with waiting logic and proper state transitions
    private void Search()
    {
        if (!isSearching)
        {
            isSearching = true;
            searchPointIndex = 0;
            agent.speed = patrolSpeed;

            if (searchPoints.Count > 0)
                agent.SetDestination(searchPoints[searchPointIndex]);
        }

        // If all search points visited, return to patrol
        if (searchPointIndex >= searchPoints.Count)
        {
            isSearching = false;
            isWaitingAtSearchPoint = false;   // Reset waiting flag
            currentState = EnemyState.Patrolling;
            GoToNextWaypoint();
            return;
        }

        // If reached current search point and not already waiting, start waiting
        if (!agent.pathPending && agent.remainingDistance <= 0.5f && !isWaitingAtSearchPoint)
        {
            StartCoroutine(WaitAtSearchPoint());
        }

        // Check for player during searching — switch to chasing if player spotted
        if (CanSeePlayer())
        {
            currentState = EnemyState.Chasing;
            searchPoints.Clear();
            lostTimer = 0f;
            isWaitingAtSearchPoint = false;  // Stop waiting if chasing now
            isSearching = false;
        }
    }

    // Updated coroutine for waiting at search points
    private IEnumerator WaitAtSearchPoint()
    {
        isWaitingAtSearchPoint = true;    // Prevent multiple waits
        agent.isStopped = true;           // Stop movement while waiting
        yield return new WaitForSeconds(searchWaitTime);
        agent.isStopped = false;          // Resume movement after wait
        searchPointIndex++;               // Advance to next search point

        if (searchPointIndex < searchPoints.Count)
        {
            agent.SetDestination(searchPoints[searchPointIndex]);
        }

        isWaitingAtSearchPoint = false;   // Clear waiting flag for next iteration
    }

    private void PrepareSearch(Vector3 origin)
    {
        searchPoints.Clear();

        // Use a per-enemy random seed to vary results (optional but good for consistency)
        int seed = gameObject.GetInstanceID() + (int)Time.time;
        System.Random rng = new System.Random(seed);

        for (int i = 0; i < searchPointsCount; i++)
        {
            float angle = rng.Next(0, 360);
            float distance = Random.Range(searchRadius * 0.5f, searchRadius);

            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            Vector3 point = origin + dir * distance;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(point, out hit, 2f, NavMesh.AllAreas))
            {
                searchPoints.Add(hit.position);
            }
        }

        if (searchPoints.Count > 0)
        {
            agent.SetDestination(searchPoints[0]);
        }

        isSearching = false;
        lostTimer = 0f;
    }


    public void ReceiveCameraAlert(Vector3 alertPosition)
    {
        if (currentState == EnemyState.Patrolling || currentState == EnemyState.Searching)
        {
            lastSeenPosition = alertPosition;
            currentState = EnemyState.Searching;
            PrepareSearch(alertPosition);
        }
    }


    protected virtual void Attack()
    {
        // Implement specific attack logic in derived classes
    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 rayOrigin = head.position + Vector3.up * 0.5f;
        Vector3 directionToPlayer = (player.transform.position - rayOrigin).normalized;
        float angle = Vector3.Angle(head.forward, directionToPlayer);

        if (angle < fieldOfView / 2f)
        {
            RaycastHit hit;
            LayerMask mask = ~LayerMask.GetMask("Ground", "NavMesh");

            if (Physics.Raycast(rayOrigin, directionToPlayer, out hit, sightRange, mask))
            {
                Debug.DrawRay(rayOrigin, directionToPlayer * sightRange, Color.red);
                if (hit.transform.CompareTag("Player"))
                {
                    lastSeenPosition = player.transform.position;
                    return true;
                }
            }
        }

        return false;
    }

    public void Alert(Vector3 alertPosition)
    {
        currentState = EnemyState.Chasing;
        agent.SetDestination(alertPosition);
        Debug.Log(gameObject.name + " is responding to the alert!");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Enemy health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died!");
        if (hasKeycard && keycardPrefab != null)
        {
            Instantiate(keycardPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    private void UpdateSpotlight()
    {
        if (fovSpotlight == null || head == null) return;

        // Match the position and direction of the head
        Vector3 offset = head.forward * spotlightOffsetForward + Vector3.up * spotlightOffsetUp;
        fovSpotlight.transform.position = head.position + offset;
        fovSpotlight.transform.rotation = Quaternion.LookRotation(head.forward);

        // Match FOV and sight range dynamically
        fovSpotlight.spotAngle = fieldOfView;
        fovSpotlight.innerSpotAngle = fieldOfView * spotlightInnerAngleFactor;
        fovSpotlight.range = sightRange;

        // Ensure color and intensity stay consistent
        fovSpotlight.color = spotlightColor;
        fovSpotlight.intensity = spotlightIntensity;
    }




    private void OnDrawGizmos()
    {
        if (head == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Vector3 fovLine1 = Quaternion.Euler(0, fieldOfView / 2, 0) * transform.forward;
        Vector3 fovLine2 = Quaternion.Euler(0, -fieldOfView / 2, 0) * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(head.position, head.position + fovLine1 * sightRange);
        Gizmos.DrawLine(head.position, head.position + fovLine2 * sightRange);
    }
}
