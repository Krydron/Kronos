using System.Collections;
using UnityEngine;

public class DisttamceCuller : MonoBehaviour
{
    [SerializeField, Range(0,50)] float distance;
    [SerializeField, Range(0, 1)] float updateSeconds;
    private Transform player;
    private Canvas canvas;

    IEnumerator distanceCull()
    {
        while (true)
        {
            if (Vector3.Distance(transform.position, player.position) > distance)
            {
                transform.Find("WorldSpaceSubtitles").gameObject.SetActive(false);
            }
            else
            {
                transform.Find("WorldSpaceSubtitles").gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(updateSeconds);
        }
    }


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        canvas = GetComponent<Canvas>();
        StartCoroutine(distanceCull());

    }
}
