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
    private GameObject[] interactables;

    private GameObject player;
    [SerializeField, Range(0,10)] float interactDistance;

    GameObject map;
    bool menuOpen;
    Pause pause;

    GameObject canInteractUI;

    //bool canInteract;
    //[SerializeField] float interactCooldown;

    /*IEnumerator interactTimer()
    {
            yield return new WaitForSeconds(interactCooldown);
            canInteract = true;
    }*/

    IEnumerator CheckDistance()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            foreach (GameObject interactable in interactables)
            {
                if (Vector3.Distance(player.transform.position, interactable.transform.position) > interactDistance)
                {
                    //hide ui
                    canInteractUI.SetActive(false);

                    continue;
                }
                //show ui
                canInteractUI.SetActive(true);
                break;
            }
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        interactables = GameObject.FindGameObjectsWithTag("Interactable");
        map = GameObject.Find("Tablet");
        map?.SetActive(false);
        menuOpen = false;
        pause = GetComponent<Pause>();
        /*canInteract = true;
        StartCoroutine(interactTimer());*/

        canInteractUI = GameObject.Find("CanInteractUI");
        canInteractUI.SetActive(false);
        StartCoroutine(CheckDistance());
    }

    public void OnMap()
    {
        if (pause.isPaused())
        {
            pause.OnPause();
        }
        else
        {
            pause.CloseMenues();
        }
        pause.SetPause(!map.activeSelf);
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
            if (Vector3.Distance(player.transform.position, interactable.transform.position) > interactDistance) { continue; }
            if (interactable.GetComponent<Interactable>().Interact()) { break; }
            Debug.Log(interactable.name);

            /*if (interactable.GetComponent<InteractableMaps>() != null)
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
                if (interactable.GetComponent<Vents>().Interact()) { break; }
            }
            if (interactable.GetComponent<KeypadInteract>() != null)
            {
                if (interactable.GetComponent<KeypadInteract>().Interact()) { break; }
            }
            if (interactable.GetComponent<VaultDoorInteract>() != null)
            {
                if (interactable.GetComponent<VaultDoorInteract>().Interact()) { break; }
            }
            if (interactable.GetComponent<NoteInteract>() != null)
            {
               if (interactable.GetComponent<NoteInteract>().Interact()) { break; }
            }*/
        }
    }
}
