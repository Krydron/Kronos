using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    [SerializeField] bool destroyOnPickup;
    [SerializeField] bool onTriggerEnter;

    public void Pickup()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().AddItemToInventory(item);
        if (destroyOnPickup) { Destroy(gameObject); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!onTriggerEnter) { return; }
        Pickup();
    }
}
