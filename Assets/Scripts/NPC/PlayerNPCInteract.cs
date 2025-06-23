using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNPCInteract : MonoBehaviour
{
    [SerializeField] private float interactRange = 3f;
    //[SerializeField] private LayerMask npcLayerMask;  // Commented out since we'll use tags here

    private Inventory playerInventory;
    private PlayerInput playerInput;
    private InputAction interactAction;

    private void Awake()
    {
        playerInventory = GetComponent<Inventory>();
        playerInput = GetComponent<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogError("[PlayerNPCInteract] PlayerInput component missing from player!");
        }
        else
        {
            interactAction = playerInput.actions["interact"];
            if (interactAction == null)
            {
                Debug.LogError("[PlayerNPCInteract] 'interact' action not found in InputActions!");
            }
        }
    }

    private void OnEnable()
    {
        if (interactAction != null)
            interactAction.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        if (interactAction != null)
            interactAction.performed -= OnInteractPerformed;
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("[PlayerNPCInteract] Interact button pressed.");
        TryInteract();
    }

    [SerializeField] private float rayHeightOffset = 1.0f;

    private void TryInteract()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * rayHeightOffset;
        Vector3 forward = transform.forward;

        Debug.DrawRay(rayOrigin, forward * interactRange, Color.yellow, 1f);

        if (Physics.Raycast(rayOrigin, forward, out RaycastHit hit, interactRange))
        {
            Debug.Log($"[PlayerNPCInteract] Raycast hit {hit.collider.name} with tag {hit.collider.tag}");
            if (hit.collider.CompareTag("NPC"))
            {
                NPCInteraction npc = hit.collider.GetComponent<NPCInteraction>();
                if (npc != null)
                {
                    npc.Interact(playerInventory);
                    return;
                }
            }
        }
        Debug.Log("[PlayerNPCInteract] No NPC detected in interact range.");
    }


    private void OnDrawGizmosSelected()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 1.5f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(rayOrigin, transform.forward * interactRange);
    }
}
