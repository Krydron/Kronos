using UnityEngine;
using System.Collections.Generic;

public class LockdownManager : MonoBehaviour
{
    public static LockdownManager Instance;

    public class LockdownZone
    {
        public Vector3 position;
        public float timestamp;

        public LockdownZone(Vector3 pos)
        {
            position = pos;
            timestamp = Time.time;
        }
    }

    private List<LockdownZone> activeLockdowns = new List<LockdownZone>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void TriggerLockdown(Vector3 position)
    {
        activeLockdowns.Add(new LockdownZone(position));
        Debug.Log("LOCKDOWN ACTIVATED at " + position);

        AlertEnemiesOfLockdown(position);
    }

    private void AlertEnemiesOfLockdown(Vector3 position)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            EnemyBase enemyScript = enemy.GetComponent<EnemyBase>();
            if (enemyScript != null)
            {
                enemyScript.EnterLockdown(position);
            }
        }
    }

    public void EndLockdown(Vector3 position)
    {
        activeLockdowns.RemoveAll(z => Vector3.Distance(z.position, position) < 0.5f);
        Debug.Log("Lockdown ended at " + position);
    }

    public void ClearAllLockdowns()
    {
        activeLockdowns.Clear();
        Debug.Log("All lockdowns cleared.");
    }

    public List<LockdownZone> GetActiveLockdowns()
    {
        return activeLockdowns;
    }

    public bool IsAnyLockdownActive()
    {
        return activeLockdowns.Count > 0;
    }
}
