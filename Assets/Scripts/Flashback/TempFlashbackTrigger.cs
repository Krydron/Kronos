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

using UnityEngine;

public class TempFlashbackTrigger : MonoBehaviour
{
    GameObject gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
    }

    private void OnTriggerEnter(Collider other)
    {
        //gameManager.GetComponent<SceneLoader>().FlashbackTrigger(1);
        Destroy(gameObject);
    }
}
