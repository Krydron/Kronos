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
        OpenDoor();
        type = DoorType.Default;
        //StartCoroutine(HoldDoorE(seconds));
    }
    
    public void LockDoor()
    {
        type = DoorType.Disabled;
        CloseDoor();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("OpenDoor", false);
    }

    public void OpenDoor()
    {
        animator = GetComponent<Animator>();
        if (animator == null) { Debug.LogError(name+"'s animator is null"); return; }
        animator.SetBool("OpenDoor", true);
    }
    public void CloseDoor()
    {
        animator = GetComponent<Animator>();
        if (animator == null) { Debug.LogError(name + "'s animator is null"); return; }
        animator.SetBool("OpenDoor", false);
    }

    public void SetLock(bool _newLockType)
    {
        throw new System.NotImplementedException("SetLock method is not implemented yet");
    }
}
