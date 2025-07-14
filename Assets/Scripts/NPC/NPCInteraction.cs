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

    public void Interact(Inventory playerInventory)
    {
        if (itemGiven)
        {
            PlayVoice(afterItemEvent, "after");
            return;
        }

        if (isMainNPC)
        {
            if (playerInventory.HasItem(requiredItem))
            {
                itemGiven = true;
                playerInventory.RemoveItem(requiredItem);
                PlayVoice(afterItemEvent, "after");
            }
            else
            {
                PlayVoice(beforeItemEvent, "before");
            }
        }
        else
        {
            if (itemGiven)
            {
                PlayVoice(afterItemEvent, "after");
            }
            else
            {
                PlayVoice(beforeItemEvent, "before");
            }
        }
    }

    private void PlayVoice(EventReference eventRef, string context)
    {
        if (eventRef.IsNull)
        {
            if (context == "before")
            {
                Debug.LogWarning($"[NPCInteraction] BEFORE voice event is missing for NPC '{gameObject.name}'.");
            }
            else if (context == "after")
            {
                Debug.LogWarning($"[NPCInteraction] AFTER voice event is missing for NPC '{gameObject.name}'.");
            }
            return;
        }

        RuntimeManager.PlayOneShot(eventRef, transform.position);
        //Debug.Log($"[NPCInteraction] Played {context.ToUpper()} FMOD event on '{gameObject.name}': {eventRef.Path}");
    }

    public bool HasReceivedItem() => itemGiven;
}
