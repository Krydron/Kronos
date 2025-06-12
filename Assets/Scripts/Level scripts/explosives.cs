using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class explosives : MonoBehaviour

{

    [SerializeField] public Transform explosion;
    [SerializeField] public float pauseTime;
    //[SerializeField] public float delay = 3f;
    //[SerializeField]float timer;
    //[SerializeField]bool hasExploded = false;
    void Start()
    {
        StartCoroutine(Pause(pauseTime));


    }


    public void Explode()
    {
        Instantiate(explosion, transform.position, explosion.rotation);
        Destroy(gameObject);
    }

    IEnumerator Pause(float time)
    {
        yield return new WaitForSeconds(time);
        Explode();
    }
























} 
   


 

    
   // void Update()
   // {
       
   // }

  

    //    timer += Time.deltaTime;
    //    if(timer <=0f)
    //    {
    //        Explode();
    //    }
    //}
    //void Explode()
    //{
    //    Debug.Log("exploded");
    //}
    //timer = delay;

