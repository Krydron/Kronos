using UnityEngine;

public class Doors : MonoBehaviour
{
    public enum DoorType
    {
        Default,
        Red,
        Blue,
        Yellow
    }
    private Animator animator;
    public DoorType type;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("OpenDoor", false);
    }

    public void OpenDoor()
    {
        if (animator == null) { Debug.Log("Doors are null"); return; }
        animator.SetBool("OpenDoor", true);
    }
    public void CloseDoor()
    {
        if (animator == null) { Debug.Log("Doors are null"); return; }
        animator.SetBool("OpenDoor", false);
    }
}
