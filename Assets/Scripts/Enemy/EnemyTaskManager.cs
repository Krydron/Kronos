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
        public float taskDuration = 5f;

        [Header("Conversation Settings")]
        public float conversationDistance = 2f;

        [Header("Task Timing")]
        [Tooltip("Time in seconds after which this task should start")]
        public float taskStartTime = 0f;

        [Header("Conversation Audio")]
        public EventReference conversationAudioEvent;
    }

    [Header("Task Queue")]
    [Tooltip("Sequence of tasks for this enemy to perform")]
    public List<TaskData> taskQueue = new List<TaskData>();

    [Tooltip("Should the task queue repeat endlessly?")]
    public bool repeatTasks = false;

    [Tooltip("Delay in seconds before repeating the entire task queue")]
    public float repeatDelay = 5f;


    private EnemyBase enemyBase;
    private NavMeshAgent agent;

    private bool isPerformingTask = false;
    private bool isConversationLeader = false; // NEW: leader flag to control conversation sequence
    private int currentTaskIndex = 0;

    private static List<EnemyTaskManager> availableForConversation = new List<EnemyTaskManager>();
    private EnemyTaskManager conversationPartner;

    private EventInstance conversationInstance;

    private void Start()
    {
        enemyBase = GetComponent<EnemyBase>();
        agent = GetComponent<NavMeshAgent>();

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

                // Wait until global elapsed time reaches this task's start time
                while (TimeTracker.Instance == null || TimeTracker.Instance.elapsedTime < task.taskStartTime)
                    yield return null;

                // Perform task based on its type
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
        // Add self to available list if not already present
        if (!availableForConversation.Contains(this))
        {
            availableForConversation.Add(this);
            Debug.Log($"{gameObject.name} added to availableForConversation.");
        }

        // Wait until partner assigned
        while (conversationPartner == null)
        {
            FindConversationPartner();

            if (conversationPartner != null)
            {
                Debug.Log($"{gameObject.name} found partner {conversationPartner.gameObject.name}.");
                break;
            }

            yield return null;
        }

        if (conversationPartner == null)
        {
            Debug.LogWarning($"{gameObject.name} could not find a conversation partner.");
            availableForConversation.Remove(this);
            yield break;
        }

        // Assign conversation leader if neither has it yet
        if (!isConversationLeader && !conversationPartner.isConversationLeader)
        {
            isConversationLeader = true;
        }

        if (isConversationLeader)
        {
            Vector3 midpoint = (transform.position + conversationPartner.transform.position) * 0.5f;
            Debug.Log($"{gameObject.name} and {conversationPartner.gameObject.name} moving to midpoint {midpoint}.");

            float timeout = 10f;
            float timer = 0f;

            bool thisArrived = false;
            bool partnerArrived = false;

            while ((!thisArrived || !partnerArrived) && timer < timeout)
            {
                timer += Time.deltaTime;

                // Keep updating destination every frame
                agent.SetDestination(midpoint);
                conversationPartner.agent.SetDestination(midpoint);

                if (!agent.pathPending)
                {
                    Debug.Log($"{gameObject.name} distance to midpoint: {agent.remainingDistance}");
                    if (agent.remainingDistance <= task.conversationDistance)
                        thisArrived = true;
                    else if (agent.pathStatus == NavMeshPathStatus.PathInvalid || agent.pathStatus == NavMeshPathStatus.PathPartial)
                        thisArrived = true; // Treat as arrived to prevent deadlock
                }

                if (!conversationPartner.agent.pathPending)
                {
                    Debug.Log($"{conversationPartner.gameObject.name} distance to midpoint: {conversationPartner.agent.remainingDistance}");
                    if (conversationPartner.agent.remainingDistance <= task.conversationDistance)
                        partnerArrived = true;
                    else if (conversationPartner.agent.pathStatus == NavMeshPathStatus.PathInvalid || conversationPartner.agent.pathStatus == NavMeshPathStatus.PathPartial)
                        partnerArrived = true; // Treat as arrived to prevent deadlock
                }

                yield return null;
            }

            if (timer >= timeout)
                Debug.LogWarning($"{gameObject.name} and {conversationPartner.gameObject.name} conversation move timed out.");

            agent.isStopped = true;
            conversationPartner.agent.isStopped = true;

            Debug.Log($"{gameObject.name} and {conversationPartner.gameObject.name} started talking.");

            PlayConversationAudio(task.conversationAudioEvent);
            conversationPartner.PlayConversationAudio(task.conversationAudioEvent);

            yield return new WaitForSeconds(task.taskDuration);

            agent.isStopped = false;
            conversationPartner.agent.isStopped = false;

            Debug.Log($"{gameObject.name} and {conversationPartner.gameObject.name} finished talking.");

            // Cleanup
            EndConversation();
            conversationPartner.EndConversation();

            isConversationLeader = false;
        }
        else
        {
            // Wait for the leader to finish conversation
            while (isConversationLeader || conversationPartner.isConversationLeader)
            {
                yield return null;
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
