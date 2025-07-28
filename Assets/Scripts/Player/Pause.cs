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

using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Pause : MonoBehaviour
{
    bool paused;
    GameObject menu;
    GameObject blur;
    GameObject[] audioSources;
    UIInteractions uIInteractions;
    FMODUnity.RuntimeManager runtimeManager;

    //Menus needed to close
    [SerializeField] List<GameObject> menues;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        paused = false;
        menu = GameObject.Find("Pause");
        menu.SetActive(false);
        blur = GameObject.Find("Blur");
        blur.SetActive(false);
        uIInteractions = GetComponent<UIInteractions>();
        //runtimeManager = 
        
    }

    public void ToggleCursor()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void PauseToggle()
    {
        audioSources = GameObject.FindGameObjectsWithTag("AudioSource");
        paused = !paused;
        //Debug.Log("Paused: " + paused);
        ToggleCursor();
        foreach (GameObject source in audioSources)
        {
            //Debug.Log("Audio Found: "+source.name);
            if (source == null) { return; }
            source.GetComponent<StudioEventEmitter>().EventInstance.setPaused(paused);
        }
        if (paused) { Time.timeScale = 0f; return; }
        Time.timeScale = 1f;
    }

    public bool isPaused()
    {
        return menu.activeSelf;
    }

    public void ToggleBlur()
    {
        blur.SetActive(paused);
    }

    public void SetPause(bool value)
    {
        if (paused == value) return;
        PauseToggle();
        ToggleBlur();
    }

    public void CloseMenues()
    {
        foreach (GameObject m in menues)
        {
            m.SetActive(false);
        }
        SetPause(false);
    }

    public void OnPause()
    {
        //hide menus
        

        //Display menu
        //menu.SetActive(!paused);
        //blur.SetActive(!paused);
        //Pause
        if (uIInteractions.MenuOpen())
        {
            uIInteractions.OnMap();
        }
        else if (!menu.activeSelf)
        {
            CloseMenues();
        }
        PauseToggle();
        menu.SetActive(paused);
        blur.SetActive(paused);
    }
}
