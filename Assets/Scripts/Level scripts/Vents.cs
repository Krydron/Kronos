/**************************************************************************************************************
* <Vents> Class
*
* The header file for the <Vents> class.
* 
* This class manages the teleporting of the player for the venting system
* 
*
* Created by: < Chrissie> 
* Date: <need to add>
* 
* Edited by: <Owen Clifton>
* Date: <02/06/25>
* Changed trigger to interact function
* Removed enabling and disabling player
* Removed player Transform variable 
* Changed playerg GameObject variable to player, set it to private and found player in code
* Changing public desitnation variable to SerializedField
* added a distance check
***************************************************************************************************************/

using UnityEngine;

public class Vents : MonoBehaviour
{
    [SerializeField] Transform destination;
    [SerializeField] float interactionDistance;
    private GameObject player;

    private void Start()
    {
        player = GameObject.Find("Player");
    }

    public bool Interact()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= interactionDistance)
        {
            player.transform.position = destination.position;
            return true;
        }
        return false;
    }
}
