using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory playerInventory = other.GetComponent<Inventory>();
            if (playerInventory)
            {
                playerInventory.AddItemToInventory(item);
                Destroy(gameObject); // Remove item from world
            }
        }
    }
}
