using Unity.VisualScripting;
using UnityEngine;

public class TabControl : MonoBehaviour
{
    [SerializeField] GameObject[] tabs;

    public void MapTab(int value)
    {
        foreach (GameObject tab in tabs)
        {
            tab.SetActive(false);
        }
        tabs[value].SetActive(true);
    }
}
