using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;

public class NoteList : MonoBehaviour
{
    [SerializeField] List<GameObject> notePages;
    [SerializeField] List<GameObject> noteButtons;
    NoteSave gameManager;
    
    /*void Start()
    {
        List<bool> list = GameObject.Find("GameManager").GetComponent<NoteSave>().LoadList();
        //if (list == null) { return; }
        for (int i = 0; i < list.Count; i++)
        {
            Debug.Log(i + " is " + list[i]);
            noteButtons[i].SetActive(list[i]);
        }
        //gameObject.SetActive(false);
    }*/

    public void LoadList(List<bool> list)
    {
        if (list == null) { Debug.Log("List is null"); }
        for (int i = 0; i < list.Count -1; i++)
        {
            //Debug.Log(i + " is " + list[i].ToString());
            noteButtons[i].SetActive(list[i]);
        }
    }

    public int NotesCount()
    {
        return noteButtons.Count;
    }

    public void Activate(int value)
    {
        noteButtons[value].SetActive(true);
        //gameManager.Activate(value);
        GameObject.Find("GameManager").GetComponent<NoteSave>().Activate(value);
    }

    public void Button(int value)
    {
        foreach (GameObject page in  notePages)
        {
            page.SetActive(false);
        }
        notePages[value].SetActive(true);
    }
}
