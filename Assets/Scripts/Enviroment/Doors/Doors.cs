using System.Collections;
using UnityEngine;

public class Doors : MonoBehaviour
{
    public enum DoorType
    {
        Default,
        Red,
        Blue,
        Yellow,
        Disabled
    }
    private Animator animator;
    public DoorType type;

    IEnumerator HoldDoorE(float seconds)
    {
        OpenDoor();
        yield return new WaitForSeconds(seconds);
        CloseDoor();
    }

    public void HoldDoor(float seconds)
    {
        StartCoroutine(HoldDoorE(seconds));
    }

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
