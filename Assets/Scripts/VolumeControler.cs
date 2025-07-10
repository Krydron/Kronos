using FMOD.Studio;
using UnityEngine;

public enum SoundBar
{
    Maater,
    Music,
    SFX,
    Dialogue
}

public class VolumeControler : MonoBehaviour
{
    VCA vca;

    public void SetBar(string name)
    {
        vca = FMODUnity.RuntimeManager.GetVCA("vca:/"+name);
    }

    public void SetVolume(float value)
    {
        vca.setVolume(value);
    }
}
