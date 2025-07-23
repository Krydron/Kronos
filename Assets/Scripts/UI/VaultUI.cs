using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using FMOD.Studio;
using FMODUnity;

public class VaultUI : MonoBehaviour
{
    [SerializeField, Range(0, 30)] List<int> sequence;
    [SerializeField] GameObject door;
    int sequencePointer;
    float rotation;
    GameObject dial;
    Slider slider;
    float value;
    TextMeshProUGUI text;
    //[SerializeField] float delay;
    bool lockedMechanism;

    Pause pause;

    [Header("Sound")]
    [SerializeField] GameObject turn;
    [SerializeField] GameObject click;


    private void OnEnable()
    {
        pause.PauseToggle();
        lockedMechanism = false;
    }

    private void OnDisable()
    {
        sequencePointer = 0;
        slider.value = 1;
    }

    private void Awake()
    {
        pause = GameObject.FindGameObjectWithTag("Player").GetComponent<Pause>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        sequencePointer = 0;
        dial = transform.Find("Dial").gameObject;
        slider = transform.Find("Slider").gameObject.GetComponent<Slider>();
        //text = transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
    }

    IEnumerator CheckSequence()
    {
        //reset check
        if (value == 1)
        {
            sequencePointer = 0;
            lockedMechanism = false;
            yield break;
        }

        

        Debug.Log("Value: "+value.ToString()+"\nSequence Value"+sequence[sequencePointer]);


        Debug.Log(sequencePointer % 2);
        if (sequencePointer != 0)
        {
            if ((sequencePointer % 2) == 1)
            {
                if (value > sequence[sequencePointer - 1])
                {
                    Debug.Log("Jam");
                    lockedMechanism = true;
                }
            }
            else
            {
                if (value < sequence[sequencePointer - 1])
                {
                    lockedMechanism = true;
                }
            }
        }

        //checking sequence
        if (value != sequence[sequencePointer]) { turn.GetComponent<StudioEventEmitter>().Play(); yield break; }
        if (lockedMechanism) { yield break; }
        click.GetComponent<StudioEventEmitter>().Play();
        //yield return new WaitForSecondsRealtime(1);
        //if (value != sequence[sequencePointer]) { yield break; }
        sequencePointer++;
        Debug.Log("Click");
        if (sequencePointer < sequence.Count) { yield break; }
        //Win condition
        Debug.Log("Win");
        pause.PauseToggle();
        sequencePointer = 0;
        gameObject.SetActive(false);
        door.GetComponent<Doors>().HoldDoor(2);
    }

    public void UpdateUI()
    {
        value = slider.value;
        rotation = (value - 1) * 12;
        //rotation = (slider.value-1) * 12.4137931f; 11.61290323
        transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text = value.ToString();
        dial.transform.eulerAngles = new Vector3(0, 0, rotation);

        StartCoroutine(CheckSequence());
    }
}
