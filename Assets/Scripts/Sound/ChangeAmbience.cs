using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using System.Data;

public class ChangeAmbience : MonoBehaviour
{
    EventInstance musicEvent;
    PARAMETER_ID paramID;
    [SerializeField, Range(0,10)] float value;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //local veriablles only needed to find the id
        EventDescription musicDesc;
        PARAMETER_DESCRIPTION paramDesc;

        //find the event instance and the parameter id from the emmiter
        musicEvent = GetComponent<StudioEventEmitter>().EventInstance;
        musicEvent.getDescription(out musicDesc);
        musicDesc.getParameterDescriptionByName("Level", out paramDesc);
        paramID = paramDesc.id;
    }

    public void Default()
    {
        musicEvent.setParameterByID(paramID, 2);
    }

    public void Combat()
    {
        musicEvent.setParameterByID(paramID, 3);
    }

    public void End()
    {
        musicEvent.setParameterByID(paramID, 10);
    }

    // Update is called once per frame
    void Update()
    {
        //musicEvent.setParameterByID(paramID, value);
    }
}
