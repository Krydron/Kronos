using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Lockdown : MonoBehaviour
{
    // doors lock
    //private Animator animator;
    public GameObject alarmLight;
    public Light lockdownLight;
    public float flickerSpeed = 1f;
    public float timer;
    float flickerDelay = 0.1f;
    [SerializeField] private List<Doors> doorsToLock = new List<Doors>();
    private void Start()
    {
        
        //animator = GetComponent<Animator>();
        //animator.SetBool("OpenDoor", true);
        lockdownLight.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //LockDoor();
            //StartCoroutine(Flicker());
        }

        
    }

    
    
    private void Awake()
    {
        if(alarmLight)
        {
            lockdownLight = GetComponent<Light>();
        }

                
    }
    public void BeginLockdown()
    {
        var doorsToLock = new List<Doors>();
        doorsToLock.Add("Door1");
        doorsToLock.Add("Door2");
        doorsToLock.Add("Door3");
        doorsToLock.Add("Door4");
        
        foreach (Doors Door in doorsToLock)
        {
            doors.SetLock(true);
            StartCoroutine(Flicker());
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

    //public void LockDoor()
    //{
    //    //if (animator == null)
            //animator.SetBool("ClosedDoor", false);
            

    //}
    //enemies spawn in
}
