using UnityEngine;

public class PlayerPickpocket : MonoBehaviour
{
    private Inventory inventory;
    private float pickpocketRange = 2f; // The range within which pickpocketing is allowed

    void Start()
    {
        inventory = GetComponent<Inventory>();
    }

    void Update()
    {
        // Detect if the player presses the pickpocket button (e.g., 'E' or right-click)
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickpocket();
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
