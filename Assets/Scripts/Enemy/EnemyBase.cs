using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using FMODUnity;
using FMOD.Studio;
using static UnityEditorInternal.VersionControl.ListControl;

public class EnemyBase : MonoBehaviour
{
    public enum EnemyState { Patrolling, Chasing, Attacking, Searching, Lockdown }
    public EnemyState currentState;

    public Transform head;
    public static List<EnemyBase> AllGuards = new List<EnemyBase>();

    private EnemyState lastState;

    [Header("Patrolling / Movement Speeds")]
    public Transform[] patrolPoints;
    private int waypointIndex;
    private bool isWaiting;
    public float patrolWaitTime = 3f;
    public float patrolSpeed = 3.5f;
    public float chaseSpeed = 5.5f;


    [Header("Detection")]
    public GameObject player;
    public float sightRange = 15f;
    public float fieldOfView = 90f;
    public float detectionTime = 2f;
    public float lostTime = 3f;
    [SerializeField] float minDistance = 2f;
    public AnimationCurve angleDetectionCurve;

    private float detectionTimer;
    private float lostTimer;
    private bool playerInSight;

    [Header("Turn Around Settings")]
    [Tooltip("Time in seconds to complete the 180° turn")]
    [SerializeField] private float turnDuration = 1f;
    [SerializeField] private float alertDelayAfterTurn = 0.5f;

    private bool isTurningAround = false;

    [SerializeField] private float alertRadius = 10f;
    public bool isAlerted = false;

    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Keycard Drop")]
    public GameObject keycardPrefab;
    public bool hasKeycard = false;

    [Header("Item Drop")]
    public GameObject itemPrefab;
    public bool hasItem = false;

    [Header("Search Behavior")]
    public float searchWaitTime = 3f;
    public float searchRadius = 6f;
    public int searchPointsCount = 5;
    private List<Vector3> searchPoints = new List<Vector3>();
    private int searchPointIndex;
    private bool isSearching;
    private Vector3 lastSeenPosition;

    private NavMeshAgent agent;
    private bool isWaitingAtSearchPoint = false;

    [Header("Spotlight Settings")]
    public Light fovSpotlight;
    public Color spotlightColor = Color.red;
    public float spotlightIntensity = 40f;
    public float spotlightOffsetForward = 0.5f;
    public float spotlightOffsetUp = 0f;
    [Range(0.1f, 1f)]
    public float spotlightInnerAngleFactor = 0.6f;

    [Header("Movement Tracking")]
    private bool isMoving = false;
    

    [Header("Studio Event Emitters")]
    public StudioEventEmitter footstepEmitter;
    public StudioEventEmitter damageEmitter;
    public StudioEventEmitter deathEmitter;

    [Header("Rotation Speeds")]
    public float patrolRotationSpeed = 2f;
    public float spottedRotationSpeed = 5f;
    public float chaseRotationSpeed = 8f;
    public float searchRotationSpeed = 3f;

    [Header("LockDown")]
    public bool lockdownActive = false;
    public GameObject GetKeycard()
    {
        return hasKeycard ? keycardPrefab : null;
    }

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
            Debug.LogWarning($"{gameObject.name} had no NavMeshAgent, one was added dynamically.");
        }

        agent.enabled = true;

        currentHealth = maxHealth;

        // Filter out null patrol points
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            List<Transform> validPoints = new List<Transform>();
            foreach (var point in patrolPoints)
            {
                if (point != null)
                    validPoints.Add(point);
            }

            patrolPoints = validPoints.ToArray();
        }

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            GoToNextWaypoint();
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} has no valid patrol points assigned. Patrolling will be skipped.", this);
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

        // Make sure footstepEmitter is stopped at start
        if (footstepEmitter != null)
        {
            footstepEmitter.Stop();
        }

        lastState = currentState;    // Initialize lastState to currentState
        CheckCombatMusic();          // Check combat music on start
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
        UpdateMovementAudio();  // Controls footstepEmitter Play/Stop

        if (currentState != lastState)
        {
            CheckCombatMusic();
            lastState = currentState;
        }
    }


    private void UpdateMovementAudio()
    {
        bool currentlyMoving = (agent != null && agent.hasPath && agent.velocity.sqrMagnitude > 0.1f && !agent.isStopped);

        if (currentlyMoving && !isMoving)
        {
            footstepEmitter.Play();
            isMoving = true;
        }
        else if (!currentlyMoving && isMoving)
        {
            footstepEmitter.Stop();
            isMoving = false;
        }
    }


    /// <summary>
    /// Returns true if the enemy is currently in the Chasing state.
    /// </summary>
    public bool IsChasingPlayer()
    {
        return currentState == EnemyState.Chasing;
    }


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

        if (CanSeePlayer())
        {
            RotateTowards(player.transform.position, spottedRotationSpeed);

            if (!isAlerted)
            {
                isAlerted = true;
                AlertNearbyGuards();
                StartChasingPlayer();
                return;
            }

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
            RotateTowards(agent.steeringTarget, patrolRotationSpeed);
            detectionTimer = Mathf.Clamp(detectionTimer - Time.deltaTime, 0, detectionTime);
        }

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

            RotateTowards(player.transform.position, chaseRotationSpeed);

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

        if (searchPointIndex >= searchPoints.Count)
        {
            isSearching = false;
            isWaitingAtSearchPoint = false;
            currentState = EnemyState.Patrolling;
            GoToNextWaypoint();
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= 0.5f && !isWaitingAtSearchPoint)
        {
            StartCoroutine(WaitAtSearchPoint());
        }

        if (CanSeePlayer())
        {
            if (!isAlerted)
            {
                isAlerted = true;
                AlertNearbyGuards();
                StartChasingPlayer();
                return;
            }

            currentState = EnemyState.Chasing;
            searchPoints.Clear();
            lostTimer = 0f;
            isWaitingAtSearchPoint = false;
            isSearching = false;
        }
        else
        {
            if (searchPoints.Count > 0)
                RotateTowards(searchPoints[searchPointIndex], searchRotationSpeed);
        }
    }

    private IEnumerator WaitAtSearchPoint()
    {
        isWaitingAtSearchPoint = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(searchWaitTime);
        agent.isStopped = false;
        searchPointIndex++;

        if (searchPointIndex < searchPoints.Count)
        {
            agent.SetDestination(searchPoints[searchPointIndex]);
        }

        isWaitingAtSearchPoint = false;
    }

    private void PrepareSearch(Vector3 origin)
    {
        searchPoints.Clear();
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

    private void RotateTowards(Vector3 targetPosition, float rotationSpeed)
    {
        Vector3 direction = (targetPosition - head.position).normalized;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            head.rotation = Quaternion.Slerp(head.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
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

        if (damageEmitter != null)
        {
            damageEmitter.Play();
        }

        BecomeAlerted();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (deathEmitter != null)
        {
            deathEmitter.Play();
        }

        if (keycardPrefab != null)
        {
            Instantiate(keycardPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }


    private void OnDestroy()
    {
        if (footstepEmitter != null)
        {
            footstepEmitter.Stop();
        }
    }

    private void UpdateSpotlight()
    {
        if (fovSpotlight == null || head == null) return;

        Vector3 offset = head.forward * spotlightOffsetForward + Vector3.up * spotlightOffsetUp;
        fovSpotlight.transform.position = head.position + offset;
        fovSpotlight.transform.rotation = Quaternion.LookRotation(head.forward);

        fovSpotlight.spotAngle = fieldOfView;
        fovSpotlight.innerSpotAngle = fieldOfView * spotlightInnerAngleFactor;
        fovSpotlight.range = sightRange;

        fovSpotlight.color = spotlightColor;
        fovSpotlight.intensity = spotlightIntensity;
    }

    private void OnEnable()
    {
        if (!AllGuards.Contains(this))
            AllGuards.Add(this);
    }

    private void OnDisable()
    {
        if (AllGuards.Contains(this))
            AllGuards.Remove(this);
    }

    public void AlertNearbyGuards()
    {
        foreach (var guard in AllGuards)
        {
            if (guard == this) continue;

            float distance = Vector3.Distance(transform.position, guard.transform.position);
            if (distance <= alertRadius)
            {
                guard.BecomeAlerted();
            }
        }
    }

    public void BecomeAlerted()
    {
        if (isAlerted) return;

        isAlerted = true;
        Debug.Log(name + " is now alerted!");
        StartChasingPlayer();
    }

    private void StartChasingPlayer()
    {
        if (player == null) return;

        lastSeenPosition = player.transform.position;
        currentState = EnemyState.Chasing;
        agent.SetDestination(lastSeenPosition);
    }

    private Coroutine turnAroundCoroutine;

    public void TurnAroundForSeconds(float duration, float delayAfterTurn)
    {
        // Only allow turning around while patrolling and not already turning
        if (!isTurningAround && currentState == EnemyState.Patrolling)
        {
            turnAroundCoroutine = StartCoroutine(TurnAroundCoroutine(duration, delayAfterTurn));
        }
    }

    private IEnumerator TurnAroundCoroutine(float turnDuration, float alertDelayAfterTurn)
    {
        isTurningAround = true;

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0f, 180f, 0f);

        float elapsed = 0f;
        while (elapsed < turnDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / turnDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;

        yield return new WaitForSeconds(alertDelayAfterTurn);

        // Confirm still patrolling before alerting — optional safety check
        if (currentState == EnemyState.Patrolling)
        {
            BecomeAlerted();
        }

        yield return new WaitForSeconds(1f);

        elapsed = 0f;
        while (elapsed < turnDuration)
        {
            transform.rotation = Quaternion.Slerp(targetRotation, startRotation, elapsed / turnDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = startRotation;

        isTurningAround = false;
    }

    public void CheckCombatMusic()
    {
        if (currentState == EnemyState.Attacking)
        {
            // Start combat music
        }
        else
        {
            // Stop combat music
        }
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
