using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeLimit : MonoBehaviour
{
    [SerializeField, Range(0,60)] int minutes;
    [SerializeField, Range(0,60)] int seconds;
    int endTime;
    int counter;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        endTime = (minutes * 60) + seconds;
    }

    IEnumerator Counter()
    {
        while (counter < endTime)
        {
            //Debug.Log("Tick");
            yield return new WaitForSeconds(1);
            counter++;
            if (counter == endTime-18)
            {
                GameObject.Find("Background Music").GetComponent<ChangeAmbience>().End();
            }
            if (counter == endTime)
            {
                GameObject.Find("Player").GetComponent<PlayerHealth>().DecrementHealth(1000);
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();
        if (scene.name != "Ship") { return; }
        //Debug.Log("Counter level: " + level);
        counter = 0;
        StartCoroutine(Counter());
    }
}
