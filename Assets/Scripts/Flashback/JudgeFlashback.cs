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
//using UnityEngine.SceneManagement;

public class JudgeFlashback : MonoBehaviour
{
    GameObject gameManager;
    GameObject eyes;

    IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(65);
        eyes.GetComponent<Eyes>().ShutEyes();
        yield return new WaitForSeconds(2);
        gameManager.GetComponent<SceneLoader>().EndFlashback(0);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1.0f;
        gameManager = GameObject.Find("GameManager");
        eyes = GameObject.Find("Eyes");
        StartCoroutine(NextLevel());
    }
}
