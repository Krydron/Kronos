using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour
{
    GameObject splash;
    static private bool isActive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        isActive = true;
        splash = GameObject.Find("Splash");
        if (splash == null) { return; }
        splash.SetActive(isActive);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainMenu") { return; }
        splash = GameObject.Find("Splash");
        SplashUpdate();
    }
    void SplashUpdate()
    {
        if (splash == null) { return; }
        splash.SetActive(isActive);
    }

    public void AnyButton()
    {
        if (isActive)
        {
            isActive = false;
            SplashUpdate();
        }
    }
}
