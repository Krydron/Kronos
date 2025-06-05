using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class Lockdown : MonoBehaviour
{
    // doors lock
    private Animator animator;

    private void Start()
    {
        
        animator = GetComponent<Animator>();
        animator.SetBool("OpenDoor", true);
        lockdownLight.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LockDoor();
            StartCoroutine(Flicker());
        }

        
    }

    public void LockDoor()
    {
        if (animator == null)
            animator.SetBool("ClosedDoor", false);
            

    }

    //alarm
    public GameObject alarmLight;
    public Light lockdownLight;
    public float flickerSpeed = 1f;
    public float timer;
    float flickerDelay = 0.1f;
    private void Awake()
    {
        if(alarmLight)
        {
            lockdownLight = GetComponent<Light>();
        }

                
    }

  
    private void OnTriggerExit(Collider other)
    {
        StopAllCoroutines();
    }
    IEnumerator Flicker()
    {
        
        WaitForSeconds delayTime = new WaitForSeconds(flickerDelay);

        while(true)
        {
            lockdownLight.enabled = !lockdownLight.enabled;
            yield return delayTime;
        }
    }
    //enemies spawn in
}
