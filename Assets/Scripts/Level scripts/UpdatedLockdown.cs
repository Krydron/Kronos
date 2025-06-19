using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatedLockdown : MonoBehaviour
{

    [SerializeField] private bool openTrigger = false;
    [SerializeField] private bool closeTrigger = false;
    [SerializeField] public Animator animator;


    [SerializeField] public GameObject alarmLight;
    [SerializeField] public Light lockdownLight;
    [SerializeField] public float flickerSpeed = 1f;
    [SerializeField] public float timer;
    [SerializeField] public float flickerDelay = 0.1f;

    // work in progress to see if delegates are easier to work with
    public delegate void lockdown();
    public static lockdown onLockdown;
    public UpdatedLockdown()
    {
        onLockdown = InitiateLockdown;
    }
    public event lockdown OnLockdown;

    public event lockdown lockdownCompleted;
    // work in progress to see if delegates are easier to work with
    private void Start()
    {
        animator = GetComponent<Animator>();
        lockdownLight.enabled = false;

        InitiateLockdown();
    }
    public void InitiateLockdown()
    {

        StartCoroutine("Flicker");
        animator.SetBool("shutters close", true);

    }
   public void LockdownEndConditions()
    {
        if(CompareTag("Player"))
        {
            
        }
    }
    public void EndLockdown()
    {
        StopAllCoroutines();
        animator.SetBool("Shutters open", true);
    }
    private void Awake()
    {
        if (alarmLight)
        {
            lockdownLight = GetComponent<Light>();
        }


    }
    IEnumerator Flicker()
    {

        WaitForSeconds delayTime = new WaitForSeconds(flickerDelay);

        while (true)
        {
            lockdownLight.enabled = !lockdownLight.enabled;
            yield return delayTime;
        }
    }
}
