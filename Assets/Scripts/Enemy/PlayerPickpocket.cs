using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickpocket : MonoBehaviour
{
    private Inventory inventory;
    private float pickpocketRange = 2f; // The range within which pickpocketing is allowed

    private PlayerInput playerInput;
    private InputAction interactAction;

    void Start()
    {
        inventory = GetComponent<Inventory>();
        playerInput = GetComponent<PlayerInput>();

        if (playerInput != null)
        {
            interactAction = playerInput.actions["Interact"];
            if (interactAction != null)
            {
                interactAction.performed += ctx => TryPickpocket();
            }
            else
            {
                Debug.LogError("Interact action not found in PlayerInput actions.");
            }
        }
        else
        {
            Debug.LogError("PlayerInput component not found.");
        }
    }

    void OnDestroy()
    {
        if (interactAction != null)
        {
            interactAction.performed -= ctx => TryPickpocket();
        }
    }

    void TryPickpocket()
    {
        if (inventory == null)
        {
            Debug.LogError("Inventory is not assigned!");
            return;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickpocketRange);

        foreach (var hitCollider in hitColliders)
        {
            EnemyBase enemy = hitCollider.GetComponent<EnemyBase>();
            if (enemy != null && enemy.hasKeycard)
            {
                GameObject keycard = enemy.GetKeycard(); // Get the keycard object from the enemy
                if (keycard != null)
                {
                    Keycard keycardScript = keycard.GetComponent<Keycard>();
                    if (keycardScript != null)
                    {
                        inventory.AddKeycardToInventory(keycardScript); // Add the keycard to the inventory
                        Debug.Log("Keycard picked from enemy!");

                        enemy.hasKeycard = false;
                        Destroy(keycard);
                    }
                    else
                    {
                        Debug.LogError("Keycard script not found on keycard object!");
                    }
                }
                else
                {
                    Debug.LogError("Keycard object is null!");
                }
            }
        }
    }
}
