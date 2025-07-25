using System.Collections.Generic;
using UnityEngine;

public class SubtitleTrigger : MonoBehaviour
{
    public enum SubtitleMode { ScreenSpace, WorldSpace }

    [System.Serializable]
    public class SubtitleLine
    {
        [TextArea]
        public string text;
        public float duration = 2f;
    }

    [Header("Subtitle Mode")]
    public SubtitleMode subtitleMode = SubtitleMode.ScreenSpace;

    [Header("Subtitle Lines")]
    public List<SubtitleLine> lines = new List<SubtitleLine>();

    [Header("World Space Options")]
    public WorldSubtitleDisplay worldSubtitleDisplay; // No longer a prefab
    public Vector3 worldOffset = Vector3.up * 2f;

    public void PlaySubtitles()
    {
        List<string> texts = new List<string>();
        List<float> durations = new List<float>();

        foreach (var line in lines)
        {
            texts.Add(line.text);
            durations.Add(line.duration);
        }

        if (subtitleMode == SubtitleMode.ScreenSpace)
        {
            if (SubtitleManager.Instance != null)
            {
                for (int i = 0; i < texts.Count; i++)
                {
                    SubtitleManager.Instance.ShowSubtitle(texts[i], durations[i]);
                }
            }
            else
            {
                Debug.LogWarning("SubtitleManager not found for screen space subtitles.");
            }
        }
        else if (subtitleMode == SubtitleMode.WorldSpace)
        {
            if (worldSubtitleDisplay == null)
            {
                Debug.LogWarning("WorldSubtitleDisplay not assigned.");
                return;
            }

            worldSubtitleDisplay.transform.position = transform.position + worldOffset;

            worldSubtitleDisplay.PlaySubtitles(texts, durations);
        }
    }
}
