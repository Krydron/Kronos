using FMOD.Studio;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum SoundBar
{
    Maater,
    Music,
    SFX,
    Dialogue
}

public class VolumeControler : MonoBehaviour
{
    string name;
    VCA vca;
    SettimgsTracker gameManager;

    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider dialogueSlider;


    private void OnEnable()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<SettimgsTracker>();
        FMODUnity.RuntimeManager.GetVCA("vca:/Master").setVolume(gameManager.masterVolume);
        FMODUnity.RuntimeManager.GetVCA("vca:/BackgroundMusic").setVolume(gameManager.musicVolume);
        FMODUnity.RuntimeManager.GetVCA("vca:/SFX").setVolume(gameManager.sfxVolume);
        FMODUnity.RuntimeManager.GetVCA("vca:/Dialogue").setVolume(gameManager.dialogueVolume);

        masterSlider.value = gameManager.masterVolume;
        musicSlider.value = gameManager.musicVolume;
        sfxSlider.value = gameManager.sfxVolume;
        dialogueSlider.value = gameManager.dialogueVolume;
    }

    public void SetBar(string name)
    {
        this.name = name;
        vca = FMODUnity.RuntimeManager.GetVCA("vca:/"+name);
    }

    public void SetVolume(float value)
    {
        vca.setVolume(value);
        switch (name)
        {
            case "Master":
                gameManager.masterVolume = value; 
                break;
            case "BackgroundMusic":
                gameManager.musicVolume = value;
                break;
            case "SFX":
                gameManager.sfxVolume = value;
                break;
            case "Dialogue":
                gameManager.dialogueVolume = value;
                break;
        } 
    }
}
