using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject subtitlePanel;
    public TextMeshProUGUI subtitleText;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        subtitleText.text = "";
        if (subtitlePanel != null)
        {
            subtitlePanel.SetActive(false);
        }
    }

    /// <summary>
    /// Plays the given lines with the matching timing.
    /// </summary>
    /// <param name="lines">List of subtitle lines.</param>
    /// <param name="lineDurations">How long to show each line (seconds).</param>
    public void PlaySubtitles(List<string> lines, List<float> lineDurations)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        if (subtitlePanel != null)
        {
            subtitlePanel.SetActive(true);
        }

        currentRoutine = StartCoroutine(SubtitleRoutine(lines, lineDurations));
    }

    private IEnumerator SubtitleRoutine(List<string> lines, List<float> lineDurations)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            subtitleText.text = lines[i];
            float duration = (i < lineDurations.Count) ? lineDurations[i] : 2.5f;
            yield return new WaitForSeconds(duration);
        }

        subtitleText.text = "";
        if (subtitlePanel != null)
        {
            subtitlePanel.SetActive(false);
        }
    }

    public void ClearSubtitles()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        subtitleText.text = "";
        if (subtitlePanel != null)
        {
            subtitlePanel.SetActive(false);
        }
    }
}
