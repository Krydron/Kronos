/**************************************************************************************************************
* <Name> Class
*
* The header file for the <Name> class.
* 
* This class 
* 
*
* Created by: <Owen Clifton> 
* Date: <need to add>
*
***************************************************************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorManager : MonoBehaviour
{
    [SerializeField] float openDistance;
    [SerializeField] uint doorUpdateSpeed = 2;
    GameObject player;
    GameObject[] doors;
    Inventory inventory;
    //Doors doorComponent;
    bool scan;


    IEnumerator CheckDoors()
    {
        doors = GameObject.FindGameObjectsWithTag("Door");
        player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<Inventory>();
        while (true)
        {
            foreach (GameObject door in doors)
            {
                Doors doorComponent;
                if (door == null) { continue; }
                doorComponent = door.GetComponent<Doors>();
                if (doorComponent == null) { continue; }
                if (Vector3.Distance(player.transform.position, door.transform.position) > openDistance)
                {
                    doorComponent.CloseDoor();
                    continue;
                }
                if (CheckKey(doorComponent))
                {
                    doorComponent.OpenDoor();
                    continue;
                }
            }
            yield return new WaitForSeconds(doorUpdateSpeed);
        }
    }

    private bool CheckKey(Doors doorComponent)
    {
        foreach (Key key in inventory.keys)
        {
            if ((doorComponent.type == key.type && key.have) || (doorComponent.type == Doors.DoorType.Default))
            {
                return true;
            }
        }
        return false;
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        //player = GameObject.FindGameObjectWithTag("Player");
        //doors = GameObject.FindGameObjectsWithTag("Door");
        //inventory = player.GetComponent<Inventory>();
        //StartCoroutine(CheckDoors());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        //StopAllCoroutines();
        if (scene.name != "Ship") { return; }
        /*player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) { return; }
        inventory = player.GetComponent<Inventory>();
        doors = GameObject.FindGameObjectsWithTag("Door");
        if (doors  == null) { return; }
        if (CheckDoors() == null) { Debug.LogError("Check doors is null"); return; }*/
        StartCoroutine(CheckDoors());

    }


    // Update is called once per frame
    void Update()
    {
        if (!scan) { return; }
        doors = GameObject.FindGameObjectsWithTag("Door");
        player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<Inventory>();
        if (doors == null) { return; }
        if (inventory == null) { return; }
        if (player == null) { return; }
        foreach (GameObject door in doors)
        {
            Doors doorComponent;
            if (door == null) { continue; }
            doorComponent = door.GetComponent<Doors>();
            if (doorComponent == null) { continue; }
            if (Vector3.Distance(player.transform.position, door.transform.position) > openDistance)
            {
                doorComponent.CloseDoor();
                continue;
            }
            if (CheckKey(doorComponent))
            {
                doorComponent.OpenDoor();
                continue;
            }
        }
    }
}
