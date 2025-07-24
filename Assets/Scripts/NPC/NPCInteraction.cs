using UnityEngine;
using FMODUnity;

public class NPCInteraction : MonoBehaviour
{
    public bool isMainNPC;

    [Header("Required Item")]
    public Item requiredItem;

    [Header("FMOD Voice Events")]
    public EventReference beforeItemEvent;
    public EventReference afterItemEvent;

    private bool itemGiven = false;

    [Header("Subtitles")]
    public SubtitleTrigger beforeItemSubtitleTrigger;
    public SubtitleTrigger afterItemSubtitleTrigger;

    public void Interact(Inventory playerInventory)
    {
        if (itemGiven)
        {
            PlayVoice(afterItemEvent, "after");
            PlaySubtitles(afterItemSubtitleTrigger);
            return;
        }

        if (isMainNPC)
        {
            if (playerInventory.HasItem(requiredItem))
            {
                itemGiven = true;
                playerInventory.RemoveItem(requiredItem);
                PlayVoice(afterItemEvent, "after");
                PlaySubtitles(afterItemSubtitleTrigger);
            }
            else
            {
                PlayVoice(beforeItemEvent, "before");
                PlaySubtitles(beforeItemSubtitleTrigger);
            }
        }
        else
        {
            if (itemGiven)
            {
                PlayVoice(afterItemEvent, "after");
                PlaySubtitles(afterItemSubtitleTrigger);
            }
            else
            {
                PlayVoice(beforeItemEvent, "before");
                PlaySubtitles(beforeItemSubtitleTrigger);
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

    private void PlaySubtitles(SubtitleTrigger trigger)
    {
        if (trigger != null)
        {
            trigger.PlaySubtitles();
        }
        else
        {
            Debug.LogWarning($"[NPCInteraction] Missing SubtitleTrigger reference on {gameObject.name}.");
        }
    }

    public bool HasReceivedItem() => itemGiven;
}
