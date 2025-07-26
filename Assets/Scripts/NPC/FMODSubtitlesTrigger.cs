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
                subtitleTrigger.PlaySubtitles();
        }

        if (hasPlayedSubtitles && !emitter.IsPlaying())
        {
            hasPlayedSubtitles = false;
        }
    }
}
