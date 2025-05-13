using UnityEngine;

public class AnyKey : MonoBehaviour
{
    bool splash;

    private void Start()
    {
        splash = true;
    }

    private void OnLevelWasLoaded(int level)
    {
        gameObject.SetActive(false);
        splash = false;
    }
    public void OnAnyKey()
    {
        //Debug.Log("Anykey");
        if (splash) { GameObject.Find("GameManager").GetComponent<Splash>().AnyButton(); splash = false; }
        
    }
}
