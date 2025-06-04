using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TimeLimit : MonoBehaviour
{
    [SerializeField, Range(0,60)] int minutes;
    [SerializeField, Range(0,60)] int seconds;
    int endTime;
    int counter;

    private void Start()
    {
        endTime = (minutes * 60) + seconds;
    }

    IEnumerator Counter()
    {
        yield return new WaitForSeconds(1);
        counter++;
        if (counter == endTime)
        {
            //Reset world
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        counter = 0;
        StartCoroutine(Counter());
    }
}
