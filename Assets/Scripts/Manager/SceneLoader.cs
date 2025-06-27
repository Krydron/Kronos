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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlashbackSwitch
{
    public bool tracker;
    public bool played;
    public int scene;

    public FlashbackSwitch(int scene)
    {
        tracker = false;
        played = false;
        this.scene = scene;
    }
}

public class SceneLoader : MonoBehaviour
{
    List<FlashbackSwitch> FlashbackSwitches = new List<FlashbackSwitch>();
    int numOfFlashbacks = 1;

    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 1; i < numOfFlashbacks + 1; i++)
        {
            FlashbackSwitches.Add(new FlashbackSwitch(i));
            Debug.Log("Initialized Flashback Trigger " + i);
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        Time.timeScale = 1.0f;
    }

    public void FlashbackTrigger(int num)
    {
        if (FlashbackSwitches[num-1].played) { return; }
        FlashbackSwitches[num-1].tracker = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Flashback()
    {
        SceneManager.LoadScene(1);
    }

    public void EndFlashback(int i)
    {
        try
        {
            FlashbackSwitches[i].tracker = false;
            FlashbackSwitches[i].played = true;
        }
        catch { Debug.Log("Trigger null"); }
        ReloadScene();
    }

    public void ReloadScene()
    {
        for (int i = 0;i < FlashbackSwitches.Count;i++)
        {
            Debug.Log("Flashback: " + i + " Tracker: "+ FlashbackSwitches[i].tracker);
            if (FlashbackSwitches[i].tracker) {
                SceneManager.LoadScene(FlashbackSwitches[i].scene+1); 
                return; }
        }
        SceneManager.LoadScene(1);
    }

    public void Play()
    {
        SceneManager.LoadScene(2);
        //ReloadScene();
    }

    public void ReturnToMainMenu()
    {
        //GameObject.Find("Player").GetComponent<Pause>().PauseToggle();
        //GameObject.Find("Player").GetComponent<Pause>().ToggleCursor();
        SceneManager.LoadScene(0);
    }
    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void Win()
    {
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(3);
    }
}
