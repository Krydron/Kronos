using UnityEngine;

[System.Serializable]
public class SettingsData
{
    public float masterVolume, musicVolume, sfxVolume, dialogueVolume,mouseSensitivityX, mouseSensitivityY,cameraFOV;

    public SettingsData(SettimgsTracker settimgs)
    {
        masterVolume = settimgs.masterVolume;
        musicVolume = settimgs.musicVolume;
        sfxVolume = settimgs.sfxVolume;
        dialogueVolume = settimgs.dialogueVolume;
        mouseSensitivityX = settimgs.mouseSensitivityX;
        mouseSensitivityY = settimgs.mouseSensitivityY;
        cameraFOV = settimgs.cameraFOV;
    }
}
