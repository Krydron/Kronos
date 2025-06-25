using UnityEngine;

public class TimeTracker : MonoBehaviour
{
    public static TimeTracker Instance { get; private set; }

    [Tooltip("Maximum allowed time before timer stops (set to 0 for unlimited).")]
    public float maxTime = 0f;

    [HideInInspector]
    public float elapsedTime = 0f;

    public bool HasReachedLimit => maxTime > 0f && elapsedTime >= maxTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (maxTime > 0f && elapsedTime >= maxTime)
        {
            elapsedTime = maxTime;
            return; // Stop updating once max is reached
        }

        elapsedTime += Time.deltaTime;
    }
}
