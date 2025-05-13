using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyBase : MonoBehaviour
{
    public enum EnemyState { Patrolling, Chasing, Attacking, Searching }
    public EnemyState currentState;
    public Transform head;

    [Header("Patrolling")]
    public Transform[] patrolPoints; // Unique patrol points per enemy
    private int waypointIndex;
    private NavMeshAgent agent;
    public float patrolWaitTime; // Time to wait at each waypoint Default 2f
    private bool isWaiting;

    [Header("Detection")]
    public GameObject player;
    public float sightRange; //default 15f
    public float fieldOfView; //default 90f
    public float detectionTime; //default 2f
    public float lostTime; //default 3f

    [SerializeField] float minDistance;

    private float detectionTimer;
    private float lostTimer;

    [Header("Health")]
    public int maxHealth; //default 100
    private int currentHealth;

    [Header("Keycard Drop")]
    public GameObject keycardPrefab;
    public bool hasKeycard = false;

    public GameObject GetKeycard()
    {
        return hasKeycard ? keycardPrefab : null;
    }

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;

        currentHealth = maxHealth;

        if (patrolPoints.Length > 0)
        {
            GoToNextWaypoint();
        }
        isWaiting = false;
        detectionTimer = 0f;
        lostTimer = 0f;

        waypointIndex = 0;
    }

    private void Update()
    {
        //HandlePlayerDetection(); // Make sure to check player detection at all times

        switch (currentState)
        {
            case EnemyState.Patrolling:
                //Debug.Log("Patroling");
                Patrol();
                break;

            case EnemyState.Chasing:
                //Debug.Log("Chasing");
                Chase();
                break;

            case EnemyState.Attacking:
                //Debug.Log("Attack");
                Attack();
                break;

            case EnemyState.Searching:
                //Debug.Log("Searching");
                Search();
                break;
            default:
                break;
        }
    }

    private void HandlePlayerDetection()
    {
        if (CanSeePlayer())
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement == null) { Debug.Log("playerMovement is null"); return; }
            detectionTimer += Time.deltaTime / playerMovement.DetectionTimeModifier; // Adjusted for sneaking

            if (detectionTimer >= detectionTime)
            {
                currentState = EnemyState.Chasing;
                detectionTimer = 0;
            }
        }
        detectionTimer = 0;
    }
    private void Patrol()
    {
        if (isWaiting) return; //  Prevent movement while waiting

        // Continuous detection check for the player
        if (CanSeePlayer())
        {
            detectionTimer += Time.deltaTime;
            if (detectionTimer >= detectionTime)
            {
                currentState = EnemyState.Chasing;
                agent.destination = player.transform.position;
                detectionTimer = 0;
                return;
            }
        }
        else
        {
            detectionTimer = Mathf.Clamp(detectionTimer - Time.deltaTime, 0, detectionTime);
        }

        //  Wait before starting new path when arriving at waypoint
        if (!agent.pathPending && agent.remainingDistance <= 0.5f && !isWaiting)
        {
            StartCoroutine(PauseBeforeNextWaypoint()); //  Now won't call multiple times
        }
    }

    private IEnumerator PauseBeforeNextWaypoint()
    {
        isWaiting = true; //  Set flag to prevent re-entry
        agent.isStopped = true; //  Stop NavMeshAgent while waiting
        yield return new WaitForSeconds(patrolWaitTime);
        GoToNextWaypoint();
        agent.isStopped = false; //  Resume movement after wait
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
            float distance = Vector3.Distance(player.transform.position, transform.position);

            if (distance >= minDistance)
            {
                agent.destination = player.transform.position;
                currentState = EnemyState.Chasing; // Keep chasing
            }
            else
            {
                currentState = EnemyState.Attacking; // Switch to attack
            }
        }
        else
        {
            currentState = EnemyState.Searching; // Lost the player, start searching
                                                 
        }
    }


    protected virtual void Attack() { }

    private void Search()
    {
        lostTimer += Time.deltaTime;
        if (CanSeePlayer())
        {
            lostTimer = 0;
            currentState = EnemyState.Chasing;
        }
        if (lostTimer >= lostTime)
        {
            lostTimer = 0;
            currentState = EnemyState.Patrolling;
            GoToNextWaypoint();
        }
    }

    private bool CanSeePlayer()
    {
        if (player == null) { Debug.Log("Play Pos is null"); return false; }

        //PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        //if (playerMovement == null) { Debug.Log("Player Movement is null"); return false; }

        float detectionRange = 15f; // Adjust detection range based on sneaking

        Vector3 rayOrigin = head.position + Vector3.up * 0.5f;
        Vector3 directionToPlayer = (player.transform.position - rayOrigin).normalized;
        float angle = Vector3.Angle(head.forward, directionToPlayer);

        if (angle < fieldOfView / 2)
        {
            RaycastHit hit;
            LayerMask layerMask = ~LayerMask.GetMask("Ground", "NavMesh");

            if (Physics.Raycast(rayOrigin, directionToPlayer, out hit, detectionRange, layerMask))
            {
                Debug.DrawRay(rayOrigin, directionToPlayer * detectionRange, Color.red);
                //This would probably be a good place to update player position
                return hit.transform.CompareTag("Player");
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

    private void OnDrawGizmos()
    {
        if (head == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Vector3 fovLine1 = Quaternion.Euler(0, fieldOfView / 2, 0) * transform.forward;
        Vector3 fovLine2 = Quaternion.Euler(0, -fieldOfView / 2, 0) * transform.forward;

        Gizmos.color = Color.red;

        int thickness = 3;
        float offset = 0.05f;

        for (int i = -thickness; i <= thickness; i++)
        {
            Vector3 offsetVector = Vector3.right * i * offset;
            for (int j = 0; j < 3; j++) // Draw the same line multiple times
            {
                Gizmos.DrawLine(head.position + offsetVector, head.position + fovLine1 * sightRange + offsetVector);
                Gizmos.DrawLine(head.position + offsetVector, head.position + fovLine2 * sightRange + offsetVector);
            }
        }
    }


}
