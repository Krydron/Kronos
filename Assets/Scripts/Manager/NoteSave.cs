using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class NoteSave : MonoBehaviour
{
    private bool[] list;
    GameObject tablet;
    NoteList notes;
    int numNotes;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        tablet = Resources.Load<GameObject>("Tablet").transform.Find("TabletNotes").gameObject;
        notes = tablet.GetComponent<NoteList>();
        numNotes = notes.NotesCount();
        GenerateList();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Ship" ) { return; }
        GameObject.Find("Canvas").transform.Find("Tablet").Find("TabletNotes").GetComponent<NoteList>().LoadList(list);
    }

    private void GenerateList()
    {
        //Debug.Log("I'm alive!!!");
        //list = new List<bool>();
        //GameObject tabletNotes = GameObject.Find("TabletNotes");
        //int numNotes = tabletNotes.GetComponent<NoteList>().NotesCount();
        list = new bool[numNotes];
        for (int i = 0; i < list.Length; i++)
        {
            list[i] = false;
        }
    }

    public bool[] LoadList()
    {
       // if (list == null) { GenerateList(); }
       return list;
    }

    public void SetList(bool[] list)
    {
        this.list = list;
    }

    public void ResetValues()
    {
        for (int i = 0; i < list.Length;i++) { list[i] = false; }
    }

    public void Activate(int value)
    {
        //if (list == null) { return; }
        list[value] = true;
    }
}
