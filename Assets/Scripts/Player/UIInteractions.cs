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

public class UIInteractions : MonoBehaviour
{
    GameObject[] interactables;
    GameObject map;
    bool menuOpen;
    Pause pause;

    private void Start()
    {
        interactables = GameObject.FindGameObjectsWithTag("Interactable");
        map = GameObject.Find("Map");
        if (map != null) { map.SetActive(false); }
        menuOpen = false;
        pause = GetComponent<Pause>();
    }

    public void OnMap()
    {
        if (pause.isPaused())
        {
            pause.OnPause();
        }
        pause.PauseToggle();
        pause.ToggleBlur();
        map.SetActive(!map.activeSelf);
        /*if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            menuOpen = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            menuOpen = false;
        }*/
        
    }

    public bool MenuOpen() { 
        if (map == null)
        {
            return false;
        }
        return map.activeSelf; 
    }

    public void OnInteract()
    {
        Debug.Log("interact");
        foreach (GameObject interactable in  interactables)
        {
            if (interactable.GetComponent<InteractableMaps>() != null)
            {
                interactable.GetComponent<InteractableMaps>().Interect();
            }
            if (interactable.GetComponent<FlashbackQuit>() != null)
            {
                interactable.GetComponent<FlashbackQuit>().Exit();
            }
            if (interactable.GetComponent<Win>() != null)
            {
                interactable.GetComponent<Win>().Interact();
            }
        }
    }
}
