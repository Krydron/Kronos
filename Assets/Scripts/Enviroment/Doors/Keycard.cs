using UnityEngine;

public class Keycard : MonoBehaviour
{
    [SerializeField] Doors.DoorType keyType;
    Inventory inventory;

    public Doors.DoorType GetKeyType()
    {
        return keyType;
    }

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    private void OnTriggerEnter(Collider other)
    {
        inventory.ActivateKeyType(keyType);
        if (keyType == Doors.DoorType.Blue) { GetComponent<FlashbackTrigger>().Trigger(); }
        Destroy(gameObject);
    }
}
