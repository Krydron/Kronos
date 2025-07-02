using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using FMODUnity;
using FMOD.Studio;

public class EnemyBase : MonoBehaviour
{
    public enum EnemyState { Patrolling, Chasing, Attacking, Searching, Lockdown }
    public EnemyState currentState;

    public Transform head;
    public static List<EnemyBase> AllGuards = new List<EnemyBase>();

    [Header("Patrolling")]
    public Transform[] patrolPoints;
    private int waypointIndex;
    private bool isWaiting;
    public float patrolWaitTime = 3f;
    public float patrolSpeed = 3.5f;

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
    private float chaseSpeed;
    private bool isWaitingAtSearchPoint = false;

    [Header("Spotlight Settings")]
    public Light fovSpotlight;
    public Color spotlightColor = Color.red;
    public float spotlightIntensity = 40f;
    public float spotlightOffsetForward = 0.5f;
    public float spotlightOffsetUp = 0f;
    [Range(0.1f, 1f)]
    public float spotlightInnerAngleFactor = 0.6f;

    [Header("Audio / Movement Tracking")]
    public EventReference footstepEvent;
    public float footstepInterval = 0.5f;
    private float footstepTimer = 0f;
    private bool isMoving = false;

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
        UpdateMovementAudio();
    }

    private void UpdateMovementAudio()
    {
        if (!footstepEvent.IsNull)
        {
            if (agent != null && agent.hasPath && agent.velocity.sqrMagnitude > 0.1f && !agent.isStopped)
            {
                if (!isMoving)
                {
                    isMoving = true;
                    footstepTimer = 0f;
                }

                footstepTimer += Time.deltaTime;
                if (footstepTimer >= footstepInterval)
                {
                    RuntimeManager.PlayOneShot(footstepEvent, transform.position);
                    footstepTimer = 0f;
                }
            }
            else
            {
                isMoving = false;
                footstepTimer = 0f;
            }
        }
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

    // (the rest of your methods like Alert, TakeDamage, Die etc. stay the same below)

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

        if (hasItem && itemPrefab != null)
        {
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
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
