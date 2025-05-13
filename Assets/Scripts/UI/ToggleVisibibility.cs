using UnityEngine;

public class ToggleVisibibility : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ToggleVisibility(GameObject gameObject)
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
