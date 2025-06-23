using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    public Text dialogueText;
    public GameObject dialoguePanel;

    public bool FinishedLine { get; private set; }

    private void Awake()
    {
        Instance = this;
        dialoguePanel.SetActive(false);
    }

    public void Show(string line)
    {
        FinishedLine = false;
        dialoguePanel.SetActive(true);
        dialogueText.text = line;
        StartCoroutine(WaitForInput());
    }

    private IEnumerator WaitForInput()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space)); // Press space to continue
        FinishedLine = true;
        dialoguePanel.SetActive(false);
    }
}
