using UnityEngine;

[CreateAssetMenu(fileName = "NPCInteractionData", menuName = "Game/NPC Interaction Data")]
public class NPCInteractionData : ScriptableObject
{
    public string npcName;

    public Item requiredItem; // Item NPC expects from player, can be null

    [TextArea(3, 10)]
    public string[] beforeItemDialogue;

    [TextArea(3, 10)]
    public string[] afterItemDialogue;
}
