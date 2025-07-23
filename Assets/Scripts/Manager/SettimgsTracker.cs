using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettimgsTracker : MonoBehaviour
{
    [Range(0, 1)] public float masterVolume, musicVolume, sfxVolume, dialogueVolume;
    [Range(0, 160)] public float mouseSensitivityX, mouseSensitivityY;
    [Range(0, 120)] public float cameraFOV;

    private void Start()
    {

    }
}
