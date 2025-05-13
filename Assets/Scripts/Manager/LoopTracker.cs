/**************************************************************************************************************
* <Name> Class
*
* The header file for the <Name> class.
* 
* This class 
* 
*
* Created by: <Owen Clifton> 
* Date: <need to add>
*
***************************************************************************************************************/

using System.Collections;
using UnityEngine;

public class LoopTracker : MonoBehaviour
{
    [SerializeField] uint maxLoops;
    AudioSource grandfatherClock;
    private ushort loop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        loop = 1;
    }

    private void OnLevelWasLoaded(int level)
    {
        if (GameObject.Find("GrandFatherClock") == null) { return; }
        grandfatherClock = GameObject.Find("GrandFatherClock").GetComponent<AudioSource>();
        StartCoroutine(playChimes());
    }

    IEnumerator playChimes()
    {
        //grandfatherClock = GameObject.Find("GrandFatherClock").GetComponent<AudioSource>();
        for (int i = 0; i < loop; i++)
        {
            grandfatherClock.Play();
            Debug.Log("Play Audio: " + loop);
            yield return new WaitForSeconds(3.5f);
        }
    }

    public void IncrementLoop()
    {
        loop++;
        //StartCoroutine(playChimes());
        Debug.Log("current loop: "+ loop);
        if (loop >= maxLoops)
        {
            //Game over screen
            Debug.Log("Game Over");
        }

    }

    public uint CurrentLoop() { return loop; }
}
