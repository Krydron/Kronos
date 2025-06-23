using System.Collections;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public NPCInteractionData npcData;

    private bool itemGiven = false;

    public void Interact(Inventory playerInventory)
    {
        Debug.Log($"[NPCInteraction] Interact called on {npcData.npcName}. ItemGiven: {itemGiven}");

        if (!itemGiven && npcData.requiredItem != null && playerInventory.HasItem(npcData.requiredItem))
        {
            Debug.Log($"[NPCInteraction] Player has required item '{npcData.requiredItem.name}'. Removing item and playing after item dialogue.");
            playerInventory.RemoveItem(npcData.requiredItem);
            itemGiven = true;
            StartCoroutine(PlayDialogue(npcData.afterItemDialogue));
        }
        else if (!itemGiven)
        {
            Debug.Log("[NPCInteraction] Player does NOT have required item or no required item set. Playing before item dialogue.");
            StartCoroutine(PlayDialogue(npcData.beforeItemDialogue));
        }
        else
        {
            Debug.Log("[NPCInteraction] Item already given. Playing after item dialogue.");
            StartCoroutine(PlayDialogue(npcData.afterItemDialogue));
        }
    }

    private IEnumerator PlayDialogue(string[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("[NPCInteraction] No dialogue lines to play.");
            yield break;
        }

        foreach (var line in lines)
        {
            Debug.Log($"[NPCInteraction] Showing dialogue: {line}");
            DialogueUI.Instance.Show(line);  // Make sure you have DialogueUI set up properly
            yield return new WaitUntil(() => DialogueUI.Instance.FinishedLine);
        }

        Debug.Log("[NPCInteraction] Dialogue finished.");
    }
}
