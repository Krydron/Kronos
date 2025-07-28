using UnityEngine;

public class MainMenu : MonoBehaviour
{
    SceneLoader sceneLoader;
    Pause pause;

    private void Start()
    {
        sceneLoader = GameObject.Find("GameManager").GetComponent<SceneLoader>();
        pause = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Pause>();
    }

    public void Play(bool fromLastSave) { sceneLoader.Play(fromLastSave); }

    public void Quit() { sceneLoader.Quit(); } 

    public void ReturnToMainMenu() { sceneLoader.ReturnToMainMenu(); }

    public void Pause() { pause.OnPause(); }
}
