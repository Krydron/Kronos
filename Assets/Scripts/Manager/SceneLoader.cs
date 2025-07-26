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
    FlashbackSwitch[] FlashbackSwitches;
    int numOfFlashbacks = 0;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FlashbackSwitches = new FlashbackSwitch[numOfFlashbacks];
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
        for (int i = 0; i < FlashbackSwitches.Length; i++)
        {
            Debug.Log("Flashback: " + i + " Tracker: "+ FlashbackSwitches[i].tracker);
            if (FlashbackSwitches[i].tracker) {
                SceneManager.LoadScene(FlashbackSwitches[i].scene+1); 
                return; }
        }
        SceneManager.LoadScene(1);
    }

    public void Play(bool fromLastSave)
    {
        GameObject gameManager = GameObject.Find("GameManager");
        int currentLoop = 1;

        SaveData data = SaveSystem.LoadSave();
        if (fromLastSave && data != null)
        {
            currentLoop = data.currentLoop;
            bool[] mapsSaved = data.mapsSaved;
            Vector3[][][] lineList = new Vector3[data.lineList.Length][][];
            for (int map = 0; map < data.lineList.Length; map++)
            {
                lineList[map] = new Vector3[data.lineList[map].Length][];
                for (int line = 0; line < data.lineList[map].Length; line++)
                {
                    lineList[map][line] = new Vector3[data.lineList[map][line].Length];
                    for (int point = 0; point < data.lineList[map][line].Length; point++)
                    {
                        Debug.Log("Line value:\n X: " + data.lineList[map][line][point][0] + "\n Y: " + data.lineList[map][line][point][1] + "\n Z: " + data.lineList[map][line][point][2]);
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
            SceneManager.LoadScene(1);
        }
        else
        {
            gameManager.GetComponent<MapSave>().ResetValues();
            gameManager.GetComponent<NoteSave>().ResetValues();
            gameManager.GetComponent<LoopTracker>().SetLoop(currentLoop);
            SceneManager.LoadScene(2);
        }

        
        
    }

    public void ReturnToMainMenu()
    {
        //GameObject.Find("Player").GetComponent<Pause>().PauseToggle();
        //GameObject.Find("Player").GetComponent<Pause>().ToggleCursor();
        SaveSystem.NewSaveSlot(GameObject.Find("GameManager"));
        SceneManager.LoadScene(0);
    }
    public void Quit()
    {
        Debug.Log("Quit");
        SaveSystem.NewSaveSlot(GameObject.Find("GameManager"));
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
