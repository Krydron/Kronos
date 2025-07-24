using System.Collections;
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

    [Header("Subtitle Lines")]
    public List<SubtitleLine> lines = new List<SubtitleLine>();

    [Header("Subtitle Mode")]
    public SubtitleMode subtitleMode = SubtitleMode.ScreenSpace;

    [Header("World Space Settings")]
    public GameObject worldSubtitlePrefab;
    public Vector3 offset = new Vector3(0, 2f, 0);

    public void PlaySubtitles()
    {
        if (subtitleMode == SubtitleMode.ScreenSpace)
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
        else if (subtitleMode == SubtitleMode.WorldSpace)
        {
            StartCoroutine(PlayWorldSubtitles());
        }
    }

    private IEnumerator PlayWorldSubtitles()
    {
        foreach (var line in lines)
        {
            GameObject instance = Instantiate(worldSubtitlePrefab, transform.position + offset, Quaternion.identity);

            // Always face camera
            instance.transform.LookAt(Camera.main.transform);
            instance.transform.Rotate(0, 180, 0);

            TMPro.TextMeshProUGUI tmp = instance.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (tmp != null)
                tmp.text = line.text;

            Destroy(instance, line.duration);
            yield return new WaitForSeconds(line.duration);
        }
    }
}
