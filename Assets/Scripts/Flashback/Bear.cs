using UnityEngine;

public class Bear : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        GetComponent<FlashbackTrigger>().Trigger();
        GetComponent<ItemPickup>().Pickup();
        Destroy(gameObject);
    }
}
