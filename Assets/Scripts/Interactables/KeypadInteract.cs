/**************************************************************************************************************
* KeypadInteract Class
*
* The header file for the KeypadInteract class.
* 
* This class allows the player to interact with it to display the keypad UI and stores the unique code for that location
* 
*
* Created by: <Owen Clifton> 
* Date: <17/06/25>
*
***************************************************************************************************************/

using UnityEngine;

public class KeypadInteract : Interactable
{
    [SerializeField] string password;
    [SerializeField] GameObject door;
    [SerializeField] GameObject keypadUI;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public override bool Interact()
    {
        Debug.Log("Test");
        keypadUI.SetActive(true);
        keypadUI.GetComponent<Keypad>().SetPassword(password, door);
        return true;
    }
}
