/**************************************************************************************************************
* <GunPickup> Class
*
* The header file for the <GunPickup> class.
* 
* This class 
* 
*
* Created by: <Owen Clifton> 
* Date: <need to add>
*
***************************************************************************************************************/

using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [SerializeField] Weapon gun;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            gun.InInventory(true);
            collision.gameObject.GetComponent<Inventory>().UpdateInventoryWeapons();
            Destroy(gameObject);
        }
    }
}
