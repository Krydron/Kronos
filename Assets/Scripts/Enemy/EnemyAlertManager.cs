using UnityEngine;
using System.Collections.Generic;

public class EnemyAlertManager : MonoBehaviour
{
    public static EnemyAlertManager Instance;

    [Header("Alert Settings")]
    public float playerGunshotAlertRadius = 20f;

    [Header("Movement Proximity Alert Settings")]
    public float playerMovementAlertRadius = 10f;
    public float playerMovementThreshold = 0.1f;

    [Header("Enemy Turn Around Settings")]
    [Tooltip("Duration in seconds the enemy will turn around")]
    [SerializeField] private float turnDuration = 1.5f;

    [Tooltip("Delay after turning around before enemy resumes normal behavior")]
    [SerializeField] private float alertDelayAfterTurn = 0.5f;

    private GameObject player;
    private Vector3 lastPlayerPosition;
    private Vector3 playerVelocity;
    private Vector3 playerPosition;     
    private float distance;             

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            lastPlayerPosition = player.transform.position;
        }
    }

    private void Update()
    {
        if (player == null) return;

        playerVelocity = (player.transform.position - lastPlayerPosition) / Time.deltaTime;
        lastPlayerPosition = player.transform.position;

        CheckPlayerMovementProximityAlerts();
    }

    /// <summary>
    /// Call this when the player fires a gun to alert nearby enemies.
    /// </summary>
    public void PlayerFiredGun()
    {
        if (player == null) return;

        playerPosition = player.transform.position;

        for (int i = EnemyBase.AllGuards.Count - 1; i >= 0; i--)
        {
            EnemyBase enemy = EnemyBase.AllGuards[i];

            if (enemy == null)
            {
                EnemyBase.AllGuards.RemoveAt(i);
                continue;
            }

            distance = Vector3.Distance(playerPosition, enemy.transform.position);
            if (distance <= playerGunshotAlertRadius)
            {
                enemy.BecomeAlerted();
            }
        }
    }

    /// <summary>
    /// Checks if player is moving close enough to make nearby enemies react.
    /// If player is sneaking (detected via PlayerMovement script), enemy turns around for a set duration.
    /// </summary>
    private void CheckPlayerMovementProximityAlerts()
    {
        if (playerVelocity.magnitude < playerMovementThreshold) return;

        playerPosition = player.transform.position;

        // Get PlayerMovement component once for sneaking check
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        bool playerIsSneaking = playerMovement != null && playerMovement.IsSneaking();

        for (int i = EnemyBase.AllGuards.Count - 1; i >= 0; i--)
        {
            EnemyBase enemy = EnemyBase.AllGuards[i];

            if (enemy == null)
            {
                EnemyBase.AllGuards.RemoveAt(i);
                continue;
            }

            distance = Vector3.Distance(playerPosition, enemy.transform.position);
            if (distance <= playerMovementAlertRadius)
            {
                if (!playerIsSneaking)
                {
                    // Player moving nearby and not sneaking — enemy turns around for turnDuration, then waits alertDelayAfterTurn
                    enemy.TurnAroundForSeconds(turnDuration, alertDelayAfterTurn);
                }
            }
        }
    }
}
