using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtitleTrigger : MonoBehaviour
{
    [System.Serializable]
    public class SubtitleLine
    {
        [TextArea]
        public string text;
        public float duration = 2f;
    }

    [Header("Subtitle Lines")]
    public List<SubtitleLine> lines = new List<SubtitleLine>();

    public void PlaySubtitles()
    {
        if (SubtitleManager.Instance == null)
        {
            Debug.LogWarning("No SubtitleManager found in scene!");
            return;
        }

        foreach (var line in lines)
        {
            SubtitleManager.Instance.ShowSubtitle(line.text, line.duration);
        }
    }
}
