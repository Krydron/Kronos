using System.Collections;
using UnityEngine;

public class Keypad : MonoBehaviour
{
    string password;
    private string input;
    private Transform screenUI;
    private Transform inputUI;
    //private Transform inputTextUI;
    private Pause pause;
    
    private GameObject door;

    private void Awake()
    {
        pause = GameObject.FindGameObjectWithTag("Player").GetComponent<Pause>();
    }

    private void Start()
    {
        input = "";
        screenUI = transform.Find("Screen");
        inputUI = screenUI.transform.Find("Input");
    }

    private void OnEnable()
    {
        pause.PauseToggle();
        pause.ToggleBlur();
    }

    private void OnDisable()
    {
        ClearUIText();
    }

    public void ClearUIText()
    {
        //Set all the input markers to false
        for (int i = 0; i < input.Length; i++)
        {
            inputUI.GetChild(i).gameObject.SetActive(false);
        }
        this.input = "";
    }

    IEnumerator ScreenChange(bool win)
    {
        if (win)
        {
            //Show green screen
            screenUI.GetChild(1).gameObject.SetActive(true);
            door.GetComponent<Doors>().HoldDoor(2);
        }
        else
        {
            //Show red screen
            screenUI.GetChild(2).gameObject.SetActive(true);
        }
        yield return new WaitForSecondsRealtime(1);
        //change to default
        screenUI.GetChild(1).gameObject.SetActive(false);
        screenUI.GetChild(2).gameObject.SetActive(false);
        //reset
        ClearUIText();
        yield return new WaitForSecondsRealtime(0.1f);
        pause.PauseToggle();
        pause.ToggleBlur();
        this.gameObject.SetActive(false);
    }

    public void SetPassword(string value, GameObject door)
    {
        password = value;
        this.door = door;
    }

    public void Input(string input)
    {
        this.input+=input;
        Debug.Log(this.input);
        inputUI.GetChild(this.input.Length-1).gameObject.SetActive(true);
        if (this.input == password)
        {
            Debug.Log("Password Confirmed");
            StartCoroutine(ScreenChange(true));
            return;
        }
        if (this.input.Length >= password.Length)
        {
            StartCoroutine(ScreenChange(false));
            Debug.Log("Error");
        }
    }
}
