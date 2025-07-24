using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager Instance { get; private set; }

    [Header("UI References")]
    public TextMeshProUGUI subtitleText;
    public Canvas subtitleCanvas;

    [Header("Settings")]
    public float fadeDuration = 0.25f;

    private Queue<SubtitleEntry> subtitleQueue = new Queue<SubtitleEntry>();
    private Coroutine currentRoutine;

    [System.Serializable]
    public class SubtitleEntry
    {
        [TextArea]
        public string text;
        public float duration = 2f;

        public SubtitleEntry(string text, float duration)
        {
            this.text = text;
            this.duration = duration;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (subtitleCanvas != null)
            subtitleCanvas.enabled = false;

        subtitleText.text = "";
    }

    public void ShowSubtitle(string text, float duration)
    {
        subtitleQueue.Clear(); // <-- clear any active subtitle
        subtitleQueue.Enqueue(new SubtitleEntry(text, duration));

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        if (subtitleCanvas != null)
            subtitleCanvas.enabled = true;

        while (subtitleQueue.Count > 0)
        {
            SubtitleEntry current = subtitleQueue.Dequeue();

            subtitleText.alpha = 0f;
            subtitleText.text = current.text;

            float t = 0;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                subtitleText.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
                yield return null;
            }

            yield return new WaitForSeconds(current.duration);

            t = 0;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                subtitleText.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
                yield return null;
            }

            subtitleText.text = "";
        }

        if (subtitleCanvas != null)
            subtitleCanvas.enabled = false;

        currentRoutine = null;
    }
}
