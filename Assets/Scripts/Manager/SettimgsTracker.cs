using UnityEngine;

public class SettimgsTracker : MonoBehaviour
{
    [Range(0, 1)] public float masterVolume, musicVolume, sfxVolume, dialogueVolume;
    [Range(0, 160)] public float mouseSensitivityX, mouseSensitivityY;
    [Range(0, 120)] public float cameraFOV;

    private void Start()
    {
        RestoreSettingsFromFile();
    }

    private void RestoreSettingsFromFile()
    {
        SettingsData data = SaveSystem.SettingsLoad();
        if (data == null) { return; }
        masterVolume = data.masterVolume;
        musicVolume = data.musicVolume;
        sfxVolume = data.sfxVolume;
        dialogueVolume = data.dialogueVolume;
        mouseSensitivityX = data.mouseSensitivityX;
        mouseSensitivityY = data.mouseSensitivityY;
        cameraFOV = data.cameraFOV;
    }
}
