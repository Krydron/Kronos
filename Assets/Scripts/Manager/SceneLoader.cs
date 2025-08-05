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

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class FlashbackSwitch
{
    public bool tracker;
    public bool played;
    public string scene;

    public FlashbackSwitch(string scene)
    {
        tracker = false;
        played = false;
        this.scene = scene;
    }
}

public class SceneLoader : MonoBehaviour
{
    static FlashbackSwitch[] flashbackSwitches;


    public FlashbackSwitch[] FlashbackSwitches() { return flashbackSwitches; }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1.0f;
    }

    public void FlashbackTrigger(Flashback flashback)
    {
        if (flashbackSwitches[(int)flashback].played) { return; }
        flashbackSwitches[(int)flashback].tracker = true;
    }

    private void InitializeFlashbackList()
    {
        //if (flashbackSwitches != null) { return; }
        flashbackSwitches = new FlashbackSwitch[(int)Flashback.Length];
        for (int i = 0; i < flashbackSwitches.Length; i++)
        {
            flashbackSwitches[i] = new FlashbackSwitch(((Flashback)i).ToString());
        }
    }

    public void JudgeFlashback()
    {
        SceneManager.LoadScene(1);
    }

    public void EndFlashback()
    {
        SceneManager.LoadScene("Ship");
    }

    public void ReloadScene()
    {
        StopAllCoroutines();
        for (int i = 0; i < flashbackSwitches.Length; i++)
        {
            Debug.Log("Flashback: " + i + " Tracker: "+ flashbackSwitches[i].tracker);
            if (!flashbackSwitches[i].tracker) { continue; }
            SceneManager.LoadScene(flashbackSwitches[i].scene); 
            flashbackSwitches[i].tracker = false ;
            flashbackSwitches[i].played = true;
            return;
        }
        SceneManager.LoadScene("Ship");
    }

    public void Play(bool fromLastSave)
    {
        //Attempt to load last save if you can not reset values and load first cutscene
        if (LoadSave(fromLastSave)) { SceneManager.LoadScene("Ship"); return; }
        SceneManager.LoadScene("Courtroom");
    }

    private bool LoadSave(bool fromLastSave)
    {
        GameObject gameManager = GameObject.Find("GameManager");
        int currentLoop = 1;

        SaveData data = SaveSystem.LoadSave();
        if (fromLastSave && data != null)
        {
            //Save data found and successfully loaded
            currentLoop = data.currentLoop;
            bool[] mapsSaved = data.mapsSaved;

            //Returning data to vector3 format
            Vector3[][][] lineList = new Vector3[data.lineList.Length][][];
            for (int map = 0; map < data.lineList.Length; map++)
            {
                if (data.lineList[map] == null) {  continue; }
                lineList[map] = new Vector3[data.lineList[map].Length][];
                for (int line = 0; line < data.lineList[map].Length; line++)
                {
                    if (data.lineList[map][line] == null) { continue; }
                    lineList[map][line] = new Vector3[data.lineList[map][line].Length];
                    for (int point = 0; point < data.lineList[map][line].Length; point++)
                    {
                        //Debug.Log("Line value:\n X: " + data.lineList[map][line][point][0] + "\n Y: " + data.lineList[map][line][point][1] + "\n Z: " + data.lineList[map][line][point][2]);
                        lineList[map][line][point].x = data.lineList[map][line][point][0];
                        lineList[map][line][point].y = data.lineList[map][line][point][1];
                        lineList[map][line][point].z = data.lineList[map][line][point][2];
                    }
                }
            }
            bool[] noteList = data.noteList;

            gameManager.GetComponent<MapSave>().MSave(mapsSaved);
            gameManager.GetComponent<MapSave>().list = lineList;
            gameManager.GetComponent<NoteSave>().SetList(noteList);
            gameManager.GetComponent<LoopTracker>().SetLoop(currentLoop);
            flashbackSwitches = data.flashbackSwitches;
            return true;
        }
        else
        {
            //Failed to find and load save data
            gameManager.GetComponent<MapSave>().ResetValues();
            gameManager.GetComponent<NoteSave>().ResetValues();
            gameManager.GetComponent<LoopTracker>().SetLoop(currentLoop);
            InitializeFlashbackList();
            return false;
        }
    }

    public void ReturnToMainMenu()
    {
        //GameObject.Find("Player").GetComponent<Pause>().PauseToggle();
        //GameObject.Find("Player").GetComponent<Pause>().ToggleCursor();
        try
        {
            SaveSystem.NewSaveSlot(gameObject);
        }
        catch { }
        SceneManager.LoadScene("MainMenu");
    }
    public void Quit()
    {
        Debug.Log("Quit");
        try
        {
            SaveSystem.NewSaveSlot(gameObject);
            SaveSystem.SettingsSave(gameObject);
        }
        catch { }
        Application.Quit();
    }

    public void Win()
    {
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Win");
    }
}
