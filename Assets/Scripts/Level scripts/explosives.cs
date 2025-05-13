using System.Collections;
using UnityEngine;

public class explosives : MonoBehaviour

{
    //public float delay = 3f;
    //float timer;
    //bool hasExploded = false;


    public Transform explosion;
    public float pauseTime;
  
    void Start()
    {
        StartCoroutine(Pause(pauseTime));
        
        
    }

    
    void Update()
    {
       
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
}
