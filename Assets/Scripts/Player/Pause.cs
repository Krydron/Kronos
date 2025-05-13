using Unity.VisualScripting;
using UnityEngine;

public class Pause : MonoBehaviour
{
    bool paused;
    GameObject menu;
    GameObject blur;
    GameObject[] audioSources;
    UIInteractions uIInteractions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        paused = false;
        menu = GameObject.Find("Pause");
        menu.SetActive(false);
        blur = GameObject.Find("Blur");
        blur.SetActive(false);
        uIInteractions = GetComponent<UIInteractions>();
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
        Debug.Log("Paused: " + paused);
        ToggleCursor();
        if (paused) { Time.timeScale = 0f; return; }
        Time.timeScale = 1f;
        foreach (GameObject source in audioSources)
        {
            if (paused)
            {
                source.GetComponent<AudioSource>().Pause();
                continue;
            }
            source.GetComponent<AudioSource>().UnPause();
        }
    }

    public bool isPaused()
    {
        return menu.activeSelf;
    }

    public void ToggleBlur()
    {
        blur.SetActive(paused);
    }

    public void OnPause()
    {
        //Display menu
        menu.SetActive(!paused);
        blur.SetActive(!paused);
        //Pause
        if (uIInteractions.MenuOpen())
        {
            uIInteractions.OnMap();
        }
        PauseToggle();
        menu.SetActive(paused);
        blur.SetActive(paused);
    }
}
