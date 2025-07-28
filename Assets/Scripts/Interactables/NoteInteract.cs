using UnityEngine;

public class NoteInteract : Interactable
{
    [SerializeField] GameObject note;
    [SerializeField] int notePosition;
    [SerializeField] bool triggerFlashback;
    NoteList notes;
    Pause pause;

    private void Start()
    {
        pause = GameObject.FindGameObjectWithTag("Player").GetComponent<Pause>();
        notes = GameObject.Find("Canvas").transform.Find("Tablet").transform.Find("TabletNotes").gameObject.GetComponent<NoteList>();
    }

    public override bool Interact()
    {
        notes.Activate(notePosition);
        note.SetActive(true);
        pause.PauseToggle();
        pause.ToggleBlur();
        if (triggerFlashback) { GetComponent<FlashbackTrigger>().Trigger(); }
        return true;
    }
}
