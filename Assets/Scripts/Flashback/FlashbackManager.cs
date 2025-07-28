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
    [SerializeField, Range(0, 120)] float delay;
    GameObject gameManager;
    GameObject eyes;

    IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(delay);
        eyes.GetComponent<Eyes>().ShutEyes();
        yield return new WaitForSeconds(2);
        gameManager.GetComponent<SceneLoader>().EndFlashback();
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
