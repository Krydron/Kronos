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

public class UIInteractions : MonoBehaviour
{
    GameObject[] interactables;
    GameObject map;
    bool menuOpen;
    Pause pause;

    bool canInteract;
    [SerializeField] float interactCooldown;

    IEnumerator interactTimer()
    {
            yield return new WaitForSeconds(interactCooldown);
            canInteract = true;
    }

    private void Start()
    {
        interactables = GameObject.FindGameObjectsWithTag("Interactable");
        map = GameObject.Find("Map");
        if (map != null) { map.SetActive(false); }
        menuOpen = false;
        pause = GetComponent<Pause>();
        canInteract = true;
        StartCoroutine(interactTimer());
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
        //if (!canInteract) { return; }
        //canInteract = false;
        //StartCoroutine(interactTimer());
        //interactables = GameObject.FindGameObjectsWithTag("Interactable");  //Code for testing
        Debug.Log("interact");
        foreach (GameObject interactable in interactables)
        {
            Debug.Log(interactable.name);
            if (interactable.GetComponent<InteractableMaps>() != null)
            {
                if (interactable.GetComponent<InteractableMaps>().Interect()) { break; }
            }
            if (interactable.GetComponent<FlashbackQuit>() != null)
            {
                if (interactable.GetComponent<FlashbackQuit>().Exit()) { break; }
            }
            if (interactable.GetComponent<Win>() != null)
            {
                if (interactable.GetComponent<Win>().Interact()) { break; }
            }
            if (interactable.GetComponent<Vents>() != null)
            {
                if (interactable.GetComponent<Vents>().Interact()) { break;}
            }
        }
    }
}
