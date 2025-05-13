using System.Collections;
using UnityEngine;

public class alarm : MonoBehaviour
{

    public float offtime;
    public bool flicker;
    public float waittime;
    public GameObject alarmLight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        flicker = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(flicker == true)
        {
            StartCoroutine(flickerfunction());
            flicker = false;

        }
    }
    IEnumerator flickerfunction()
    {
        waittime = Random.Range(0.1f, 3f);
        yield return new WaitForSeconds(waittime);
        alarmLight.SetActive(false);
        yield return new WaitForSeconds(offtime);
        alarmLight.SetActive(true);
        flicker = true;

    }
}
