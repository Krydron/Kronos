using UnityEngine;
using FMODUnity;
using System.Collections.Generic;

public class NPCInteraction : MonoBehaviour
{
    public bool isMainNPC;

    [Header("Required Item")]
    public Item requiredItem;

    [Header("FMOD Voice Events")]
    public EventReference beforeItemEvent;
    public EventReference afterItemEvent;

    [Header("Subtitles")]
    [TextArea] public List<string> beforeItemSubtitles;
    public List<float> beforeItemDurations = new List<float>();

    [TextArea] public List<string> afterItemSubtitles;
    public List<float> afterItemDurations = new List<float>();

    private bool itemGiven = false;

    public void Interact(Inventory playerInventory)
    {
        if (itemGiven)
        {
            PlayVoice(afterItemEvent, "after");
            ShowSubtitles(afterItemSubtitles, afterItemDurations);
            return;
        }

        if (isMainNPC)
        {
            if (playerInventory.HasItem(requiredItem))
            {
                itemGiven = true;
                playerInventory.RemoveItem(requiredItem);
                PlayVoice(afterItemEvent, "after");
                ShowSubtitles(afterItemSubtitles, afterItemDurations);
            }
            else
            {
                PlayVoice(beforeItemEvent, "before");
                ShowSubtitles(beforeItemSubtitles, beforeItemDurations);
            }
        }
        else
        {
            if (itemGiven)
            {
                PlayVoice(afterItemEvent, "after");
                ShowSubtitles(afterItemSubtitles, afterItemDurations);
            }
            else
            {
                PlayVoice(beforeItemEvent, "before");
                ShowSubtitles(beforeItemSubtitles, beforeItemDurations);
            }
        }
    }

    private void PlayVoice(EventReference eventRef, string context)
    {
        if (eventRef.IsNull)
        {
            Debug.LogWarning($"[NPCInteraction] {context.ToUpper()} voice event missing for NPC '{gameObject.name}'.");
            return;
        }

        RuntimeManager.PlayOneShot(eventRef, transform.position);
    }

    private void ShowSubtitles(List<string> lines, List<float> durations)
    {
        if (lines != null && lines.Count > 0)
        {
            SubtitleManager.Instance.PlaySubtitles(lines, durations);
        }
    }

    public bool HasReceivedItem() => itemGiven;
}
