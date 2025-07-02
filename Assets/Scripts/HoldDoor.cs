using UnityEngine;

public class HoldDoor : MonoBehaviour
{
    [SerializeField] GameObject door; 

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            door.GetComponent<Doors>().LockDoor();
        }
    }
}
