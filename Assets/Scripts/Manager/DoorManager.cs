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
    Doors doorComponent;

    IEnumerator CheckDoors()
    {
        if (doors == null) { yield break; }
        if (inventory == null) { yield break; }
        while (true)
        {
            foreach (GameObject door in doors)
            {
                if (door == null) { continue; }
                doorComponent = door.GetComponent<Doors>();
                if (Vector3.Distance(player.transform.position, door.transform.position) > openDistance)
                {
                    doorComponent.CloseDoor();
                    continue;
                }
                if (CheckKey())
                {
                    doorComponent.OpenDoor();
                    continue;
                }
            }
            yield return new WaitForSeconds(doorUpdateSpeed);
        }
    }

    private bool CheckKey()
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
        if (scene.name != "Ship") { return; }
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) { return; }
        doors = GameObject.FindGameObjectsWithTag("Door");
        inventory = player.GetComponent<Inventory>();
        StartCoroutine(CheckDoors());
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
