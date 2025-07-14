using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class explosives : MonoBehaviour

{
    [SerializeField, Range(0, 60)] float time;

    //pickup/inventory code
    //throw code
    /*private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "exploding wall")
        {
            Interact();
        }
    }*/

    public void Interact() {
        //Check for inventory
        //Decrement bombs
        //Show bomb model
        Destroy(gameObject,time);
    })



}




//[SerializeField] public Transform explosion;
//[SerializeField] public float pauseTime;
//[SerializeField] public float delay = 3f;
//[SerializeField]float timer;
//[SerializeField]bool hasExploded = false;
// StartCoroutine(Pause(pauseTime));
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
// public void Explode()
// {
// Instantiate(explosion, transform.position, explosion.rotation);
// Destroy(gameObject);
//}

//IEnumerator Pause(float time)
//{
// yield return new WaitForSeconds(time);
// Explode();
// }
