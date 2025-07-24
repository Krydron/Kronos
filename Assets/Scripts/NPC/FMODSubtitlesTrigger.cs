using UnityEngine;
using FMODUnity;

[RequireComponent(typeof(StudioEventEmitter))]
public class FMODSubtitlesTrigger : MonoBehaviour
{
    public SubtitleTrigger subtitleTrigger;

    private StudioEventEmitter emitter;
    private bool hasPlayedSubtitles = false;

    void Awake()
    {
        emitter = GetComponent<StudioEventEmitter>();
    }

    void Update()
    {
        if (emitter != null && emitter.IsPlaying() && !hasPlayedSubtitles)
        {
            hasPlayedSubtitles = true;

            if (subtitleTrigger != null)
            {
                subtitleTrigger.PlaySubtitles();
            }
            else
            {
                Debug.LogWarning($"[FMODSubtitlesTrigger] No SubtitleTrigger assigned on {gameObject.name}");
            }
        }

        // Reset if emitter stops, so it can be triggered again
        if (hasPlayedSubtitles && !emitter.IsPlaying())
        {
            hasPlayedSubtitles = false;
        }
    }
}
