using FMODUnity;
using UnityEngine;

public class Tablet : MonoBehaviour
{
    [SerializeField] StudioEventEmitter openSound;
    [SerializeField] StudioEventEmitter closeSound;

    private void OnEnable()
    {
        openSound.Play();
    }

    private void OnDisable()
    {
        closeSound.Play();
    }
}
