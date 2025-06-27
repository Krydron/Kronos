using UnityEngine;
using System.Collections.Generic;

public class NoteSave : MonoBehaviour
{
    private List<bool> list = new List<bool>();
    GameObject tablet;
    NoteList notes;
    int numNotes;

    private void Start()
    {
        tablet = Resources.Load<GameObject>("Tablet").transform.Find("TabletNotes").gameObject;
        notes = tablet.GetComponent<NoteList>();
        numNotes = notes.NotesCount();
        GenerateList();
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level != 1 ) { return; }
        GameObject.Find("Canvas").transform.Find("Tablet").Find("TabletNotes").GetComponent<NoteList>().LoadList(list);
    }

    private void GenerateList()
    {
        Debug.Log("I'm alive!!!");
        //list = new List<bool>();
        //GameObject tabletNotes = GameObject.Find("TabletNotes");
        //int numNotes = tabletNotes.GetComponent<NoteList>().NotesCount();
        for (int i = 0; i < numNotes; i++)
        {
            list.Add(false);
        }
    }

    public List<bool> LoadList()
    {
       // if (list == null) { GenerateList(); }
       return list;
    }

    public void Activate(int value)
    {
        //if (list == null) { return; }
        list[value] = true;
    }
}
