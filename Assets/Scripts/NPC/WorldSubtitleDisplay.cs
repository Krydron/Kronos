using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldSubtitleDisplay : MonoBehaviour
{
    public TextMeshProUGUI subtitleText;
    public float fadeDuration = 0.25f;

    private Coroutine currentRoutine;

    // Set placeholder text in editor
    void Reset()
    {
        if (subtitleText != null)
        {
            subtitleText.text = "Lorem Ipsum";
            subtitleText.alpha = 1f;
        }
    }

    // Keep placeholder visible in editor when properties change
    void OnValidate()
    {
        if (subtitleText != null)
        {
            if (string.IsNullOrEmpty(subtitleText.text))
                subtitleText.text = "Lorem Ipsum";
            subtitleText.alpha = 1f;
        }
    }

    // Clear placeholder at runtime start
    void Awake()
    {
        if (Application.isPlaying)
        {
            subtitleText.text = "";
            subtitleText.alpha = 0f;
        }
    }

    public void PlaySubtitles(List<string> lines, List<float> durations)
    {
        if (!gameObject.activeInHierarchy) return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(SubtitleRoutine(lines, durations));
    }

    private IEnumerator SubtitleRoutine(List<string> lines, List<float> durations)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            subtitleText.text = lines[i];
            subtitleText.alpha = 0f;

            float t = 0;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                subtitleText.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
                yield return null;
            }

            yield return new WaitForSeconds(durations[i]);

            t = 0;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                subtitleText.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
                yield return null;
            }

            subtitleText.text = "";
        }

        subtitleText.text = "";
        subtitleText.alpha = 0f;
    }
}
