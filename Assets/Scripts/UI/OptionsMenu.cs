using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] GameObject controlSchemeMenu;

    private void OnEnable()
    {
        controlSchemeMenu.SetActive(false);
    }
}
