using UnityEngine;

public class SecretDoor : Interactable
{
    Doors door;

    private void Start()
    {
        door = GetComponent<Doors>();
    }

    private void CloseDoor()
    {
        door.CloseDoor();
    }

    public override bool Interact()
    {
        door.OpenDoor();
        Invoke("CloseDoor", 2);
        return true;
    }
}
