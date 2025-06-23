using UnityEngine;

public class NoteInteract : MonoBehaviour
{
    [SerializeField] GameObject note;
    Pause pause;

    private void Start()
    {
        pause = GameObject.FindGameObjectWithTag("Player").GetComponent<Pause>();
    }

    public bool Interact()
    {
        note.SetActive(true);
        pause.PauseToggle();
        pause.ToggleBlur();
        return true;
    }
}
