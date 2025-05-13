/**************************************************************************************************************
* <Name> Class
*
* The header file for the <Name> class.
* 
* This class 
* 
*
* Created by: <Kry> 
* Date: <need to add>
*
***************************************************************************************************************/

using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class EnemyTaskManager : MonoBehaviour
{
    public enum TaskType { None, CoffeeBreak, Conversation }
    public TaskType assignedTask = TaskType.None;

    private EnemyBase enemyBase;
    private NavMeshAgent agent;
    private bool isPerformingTask = false;

    [Header("Coffee Break Task")]
    public Transform coffeeBreakLocation;
    public float taskDuration = 5f;

    [Header("Conversation Task")]
    public float conversationDistance = 2f;
    private static List<EnemyTaskManager> availableForConversation = new List<EnemyTaskManager>();
    private EnemyTaskManager conversationPartner;

    private void Start()
    {
        enemyBase = GetComponent<EnemyBase>();
        agent = GetComponent<NavMeshAgent>();

        if (assignedTask != TaskType.None)
        {
            Invoke(nameof(StartTask), 2f); // Start after delay
        }
    }

    public void StartTask()
    {
        if (isPerformingTask || assignedTask == TaskType.None) return;

        Debug.Log($"{gameObject.name} is starting task: {assignedTask}");
        isPerformingTask = true;
        enemyBase.currentState = EnemyBase.EnemyState.Patrolling; // Keep in patrol state for now

        switch (assignedTask)
        {
            case TaskType.CoffeeBreak:
                StartCoroutine(PerformCoffeeBreak());
                break;
            case TaskType.Conversation:
                StartCoroutine(PerformConversation());
                break;
            default:
                isPerformingTask = false;
                break;
        }
    }

    private IEnumerator PerformCoffeeBreak()
    {
        if (coffeeBreakLocation == null)
        {
            Debug.LogWarning($"{gameObject.name} has no coffee break location assigned.");
            isPerformingTask = false;
            yield break;
        }

        Debug.Log($"{gameObject.name} is moving to coffee break location.");
        agent.SetDestination(coffeeBreakLocation.position);

        while (agent.pathPending || agent.remainingDistance > 0.5f)
        {
            yield return null;
        }

        // **Stop movement and perform task**
        Debug.Log($"{gameObject.name} has reached coffee break location. Taking a break...");
        agent.isStopped = true; // Stop the agent from moving
        yield return new WaitForSeconds(taskDuration); // Wait for task duration

        agent.isStopped = false; // Resume movement
        EndTask();
    }

    private IEnumerator PerformConversation()
    {
        availableForConversation.Add(this);

        while (conversationPartner == null)
        {
            yield return null; // Wait until a partner is found
            FindConversationPartner();
        }

        if (conversationPartner == null)
        {
            Debug.LogWarning($"{gameObject.name} could not find a conversation partner.");
            availableForConversation.Remove(this);
            isPerformingTask = false;
            yield break;
        }

        Vector3 midpoint = (transform.position + conversationPartner.transform.position) / 2;

        Debug.Log($"{gameObject.name} and {conversationPartner.gameObject.name} are moving to talk at {midpoint}");
        agent.SetDestination(midpoint);
        conversationPartner.agent.SetDestination(midpoint);

        while (agent.pathPending || agent.remainingDistance > conversationDistance ||
               conversationPartner.agent.pathPending || conversationPartner.agent.remainingDistance > conversationDistance)
        {
            yield return null;
        }

        // **Stop movement and "talk"**
        agent.isStopped = true;
        conversationPartner.agent.isStopped = true;
        Debug.Log($"{gameObject.name} and {conversationPartner.gameObject.name} are talking...");

        yield return new WaitForSeconds(taskDuration);

        agent.isStopped = false;
        conversationPartner.agent.isStopped = false;

        EndTask();
        conversationPartner.EndTask();
    }

    private void FindConversationPartner()
    {
        foreach (var enemy in availableForConversation)
        {
            if (enemy != this && enemy.conversationPartner == null)
            {
                conversationPartner = enemy;
                enemy.conversationPartner = this;
                availableForConversation.Remove(enemy);
                availableForConversation.Remove(this);
                break;
            }
        }
    }

    private void EndTask()
    {
        Debug.Log($"{gameObject.name} finished task and is resuming patrol.");
        isPerformingTask = false;
        assignedTask = TaskType.None;
        enemyBase.currentState = EnemyBase.EnemyState.Patrolling;
    }
}
