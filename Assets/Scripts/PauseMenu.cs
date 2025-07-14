using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    GameObject menu;
    private void Start()
    {
        menu = transform.Find("Pause").gameObject;
    }
    private void OnEnable()
    {
        menu.SetActive(true);
    }
}
