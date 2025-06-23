/**************************************************************************************************************
* VaultDoorInteract Class
*
* The header file for the VaultDoorInteract class.
* 
* This class allows the player to interact with it to display the safe dail UI
* 
*
* Created by: <Owen Clifton> 
* Date: <17/06/25>
*
***************************************************************************************************************/

using UnityEngine;

public class VaultDoorInteract : MonoBehaviour
{
    [SerializeField] GameObject UI;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //UI = GameObject.FindGameObjectWithTag
    }

    public bool Interact()
    {
        if (UI == null) { Debug.Log("UI is null"); }
        UI.SetActive(true);
        return true;
    }
}
