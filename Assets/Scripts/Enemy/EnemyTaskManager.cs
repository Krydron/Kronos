using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;

public class EnemyTaskManager : MonoBehaviour
{
    public enum TaskType { None, CoffeeBreak, Conversation }

    [System.Serializable]
    public class TaskData
    {
        public TaskType taskType = TaskType.None;

        [Header("Coffee Break Settings")]
        public Transform coffeeBreakLocation;

        [Header("Conversation Settings")]
        public float conversationDistance = 2f;
        public float taskDuration = 5f; // Shared for both task types

        [Header("Task Timing")]
        [Tooltip("Time in seconds after which this task should start")]
        public float taskStartTime = 0f;

        [Header("Conversation Audio")]
        public EventReference conversationAudioEvent;
    }

    [Header("Task Queue")]
    public List<TaskData> taskQueue = new List<TaskData>();
    public bool repeatTasks = false;
    public float repeatDelay = 5f;

    private EnemyBase enemyBase;
    private NavMeshAgent agent;
    private bool isPerformingTask = false;
    private bool isConversationLeader = false;
    private int currentTaskIndex = 0;

    private static List<EnemyTaskManager> availableForConversation = new List<EnemyTaskManager>();
    private EnemyTaskManager conversationPartner;

    private EventInstance conversationInstance;

    [Header("Subtitle Settings")]
    public float playerSubtitleDistance = 10f;
    private Transform playerTransform;

    private void Start()
    {
        enemyBase = GetComponent<EnemyBase>();
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (taskQueue.Count > 0)
        {
            StartCoroutine(TaskQueueRoutine());
        }
    }

    private IEnumerator TaskQueueRoutine()
    {
        do
        {
            for (currentTaskIndex = 0; currentTaskIndex < taskQueue.Count; currentTaskIndex++)
            {
                var task = taskQueue[currentTaskIndex];

                while (TimeTracker.Instance == null || TimeTracker.Instance.elapsedTime < task.taskStartTime)
                    yield return null;

                yield return PerformTask(task);
            }

            if (repeatTasks)
            {
                Debug.Log($"{gameObject.name} waiting {repeatDelay} seconds before repeating tasks.");
                yield return new WaitForSeconds(repeatDelay);
            }

        } while (repeatTasks);
    }

    private IEnumerator PerformTask(TaskData task)
    {
        isPerformingTask = true;
        enemyBase.currentState = EnemyBase.EnemyState.Patrolling;

        switch (task.taskType)
        {
            case TaskType.CoffeeBreak:
                yield return PerformCoffeeBreak(task);
                break;

            case TaskType.Conversation:
                yield return PerformConversation(task);
                break;

            default:
                Debug.LogWarning($"{gameObject.name} has unknown or None task type.");
                break;
        }

        isPerformingTask = false;
    }

    private IEnumerator PerformCoffeeBreak(TaskData task)
    {
        if (task.coffeeBreakLocation == null)
        {
            Debug.LogWarning($"{gameObject.name} has no coffee break location assigned.");
            yield break;
        }

        Debug.Log($"{gameObject.name} is moving to coffee break location.");
        agent.SetDestination(task.coffeeBreakLocation.position);

        while (agent.pathPending || agent.remainingDistance > 0.5f)
            yield return null;

        Debug.Log($"{gameObject.name} has reached coffee break location. Taking a break...");
        agent.isStopped = true;
        yield return new WaitForSeconds(task.taskDuration);
        agent.isStopped = false;
    }

    private IEnumerator PerformConversation(TaskData task)
    {
        if (!availableForConversation.Contains(this))
            availableForConversation.Add(this);

        // Wait to find a partner
        float waitTime = 5f;
        float waitTimer = 0f;
        while (conversationPartner == null && waitTimer < waitTime)
        {
            FindConversationPartner();
            waitTimer += Time.deltaTime;
            yield return null;
        }

        if (conversationPartner == null || conversationPartner.agent == null)
        {
            Debug.LogWarning($"{gameObject.name} could not find a valid conversation partner.");
            availableForConversation.Remove(this);
            yield break;
        }

        if (!isConversationLeader && !conversationPartner.isConversationLeader)
            isConversationLeader = true;

        if (isConversationLeader)
        {
            // Use safe midpoint
            Vector3 rawMidpoint = (transform.position + conversationPartner.transform.position) * 0.5f;
            Vector3 midpoint = FindSafeMidpointOnNavMesh(rawMidpoint, 2f, 12);

            // Validate partner again
            if (conversationPartner == null || conversationPartner.agent == null)
            {
                Debug.LogWarning("Conversation partner became invalid during midpoint calc.");
                yield break;
            }

            float timeout = 10f;
            float timer = 0f;
            bool thisArrived = false;
            bool partnerArrived = false;

            while ((!thisArrived || !partnerArrived) && timer < timeout)
            {
                timer += Time.deltaTime;

                if (agent != null && agent.isOnNavMesh)
                    agent.SetDestination(midpoint);

                if (conversationPartner.agent != null && conversationPartner.agent.isOnNavMesh)
                    conversationPartner.agent.SetDestination(midpoint);

                if (!agent.pathPending)
                    thisArrived = agent.remainingDistance <= task.conversationDistance ||
                                  agent.pathStatus == NavMeshPathStatus.PathPartial ||
                                  agent.pathStatus == NavMeshPathStatus.PathInvalid;

                if (!conversationPartner.agent.pathPending)
                    partnerArrived = conversationPartner.agent.remainingDistance <= task.conversationDistance ||
                                     conversationPartner.agent.pathStatus == NavMeshPathStatus.PathPartial ||
                                     conversationPartner.agent.pathStatus == NavMeshPathStatus.PathInvalid;

                yield return null;
            }

            if (timer >= timeout)
                Debug.LogWarning("Conversation move timed out.");

            if (agent != null) agent.isStopped = true;
            if (conversationPartner.agent != null) conversationPartner.agent.isStopped = true;

            PlayConversationAudio(task.conversationAudioEvent);
            conversationPartner.PlayConversationAudio(task.conversationAudioEvent);
            TryTriggerSubtitleIfPlayerClose();

            yield return new WaitForSeconds(task.taskDuration);

            if (agent != null) agent.isStopped = false;
            if (conversationPartner.agent != null) conversationPartner.agent.isStopped = false;

            EndConversation();
            if (conversationPartner != null)
                conversationPartner.EndConversation();

            isConversationLeader = false;
        }
        else
        {
            while (conversationPartner != null && (isConversationLeader || conversationPartner.isConversationLeader))
                yield return null;
        }
    }


    private Vector3 FindSafeMidpointOnNavMesh(Vector3 midpoint, float radius = 2f, int sampleCount = 8)
    {
        List<Vector3> candidates = new List<Vector3>();
        List<float> distances = new List<float>();

        for (int i = 0; i < sampleCount; i++)
        {
            float angle = (360f / sampleCount) * i;
            Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;
            Vector3 samplePoint = midpoint + offset;

            if (NavMesh.SamplePosition(samplePoint, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                candidates.Add(hit.position);
                distances.Add(Vector3.Distance(midpoint, hit.position));
            }
        }

        if (candidates.Count == 0)
        {
            Debug.LogWarning("No valid NavMesh candidates near midpoint. Falling back.");
            return midpoint;
        }

        float bestDist = float.MaxValue;
        Vector3 bestPoint = candidates[0];

        for (int i = 0; i < candidates.Count; i++)
        {
            if (distances[i] < bestDist)
            {
                bestDist = distances[i];
                bestPoint = candidates[i];
            }
        }

        return bestPoint;
    }

    private void TryTriggerSubtitleIfPlayerClose()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= playerSubtitleDistance)
        {
            SubtitleTrigger trigger = GetComponent<SubtitleTrigger>();
            if (trigger != null)
            {
                trigger.PlaySubtitles();
            }
        }
    }

    private void PlayConversationAudio(EventReference audioEvent)
    {
        if (audioEvent.IsNull) return;

        conversationInstance = RuntimeManager.CreateInstance(audioEvent);
        RuntimeManager.AttachInstanceToGameObject(conversationInstance, gameObject);
        conversationInstance.start();
    }

    private void FindConversationPartner()
    {
        if (conversationPartner != null) return;

        foreach (var enemy in availableForConversation)
        {
            if (enemy != this && enemy.conversationPartner == null)
            {
                conversationPartner = enemy;
                enemy.conversationPartner = this;

                availableForConversation.Remove(enemy);
                availableForConversation.Remove(this);

                Debug.Log($"{gameObject.name} paired with {enemy.gameObject.name} for conversation.");
                break;
            }
        }
    }

    private void EndConversation()
    {
        conversationPartner = null;
        isConversationLeader = false;

        if (availableForConversation.Contains(this))
            availableForConversation.Remove(this);
    }
}
