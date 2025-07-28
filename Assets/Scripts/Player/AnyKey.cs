using UnityEngine;
using UnityEngine.SceneManagement;

public class AnyKey : MonoBehaviour
{
    bool splash;

    private void Start()
    {
        splash = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainMenu") { return; }
        gameObject.SetActive(false);
        splash = false;
    }
    public void OnAnyKey()
    {
        //Debug.Log("Anykey");
        if (splash) { GameObject.Find("GameManager").GetComponent<Splash>().AnyButton(); splash = false; }
        
    }
}
