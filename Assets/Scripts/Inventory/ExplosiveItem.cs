using UnityEngine;

public class ExplosiveItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GetComponent<FlashbackTrigger>().Trigger();
        GetComponent<ItemPickup>().Pickup();
    }
}
