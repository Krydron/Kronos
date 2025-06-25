
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

using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    Inventory inventory;
    Weapon weapon;
    GameObject camera;
    [SerializeField] float shootDelay;
    float shootTime;
    AmmoDisplay ammoDisplay;
    PlayerTakedown playerTakedown;

    private void Start()
    {
        inventory = GetComponent<Inventory>();
        camera = GameObject.Find("Main Camera");
        ammoDisplay = GameObject.Find("AmmoDisplay").GetComponent<AmmoDisplay>();
        //weapon = inventory.GetWeapon();
        //ammoDisplay.UpdatDisplay(weapon.Ammo(), weapon.Rounds());
        playerTakedown = GetComponent<PlayerTakedown>(); // get reference
    }

    public void OnAttack()
    {
        if (GetComponent<UIInteractions>().MenuOpen() || GetComponent<UIInteractions>() == null || Time.timeScale == 0f) { return; }
        if (shootTime +  shootDelay > Time.time) { return; }
        //get gun details from inventroy;
        weapon = inventory.GetWeapon();
        if (weapon == null ) { Debug.Log("weapon null"); return; }

        // Try takedown first if melee weapon
        if (weapon.name == "Melee" && playerTakedown != null)
        {
            Debug.Log("Attempting takedown");
            bool takedownDone = playerTakedown.TryTakedown();
            Debug.Log("Takedown result: " + takedownDone);
            if (takedownDone)
            {
                Debug.Log("Takedown performed, skipping normal attack");
                return;
            }
        }

        if (weapon.name == "Melee")
        {
            Debug.Log("Punch");
            //raycast from player
            if (Physics.Raycast(transform.position, camera.transform.forward, out RaycastHit meleeHit, 2))
            {
                OnHit(meleeHit);
            }
            return;
        }

        if (weapon.Ammo() <= 0)
        {
            Debug.Log("Out of ammo");
            return;
        }
        weapon.Ammo(weapon.Ammo()-1);
        Debug.Log("Shoot");
        //raycast from player
        if (Physics.Raycast(transform.position, camera.transform.forward, out RaycastHit hit))
        {
            OnHit(hit);
        }
        shootTime = Time.time;
        if (ammoDisplay != null)
        {
            ammoDisplay.UpdatDisplay(weapon.Ammo(),weapon.Rounds());
        }
        //TODO: When enemy hit decrement health
    }
    public void OnReload()
    {
        if (weapon.Rounds() <= 0)
        {
            Debug.Log("No Rounds");
            return;
        }
        if (weapon.MaxAmmo() > weapon.Rounds())
        {
            weapon.Ammo(weapon.Ammo() + weapon.Rounds());
            weapon.Rounds(0);
        }
        else
        {
            weapon.Rounds(weapon.Rounds() - (weapon.MaxAmmo() - weapon.Ammo()));
            weapon.Ammo(weapon.MaxAmmo());
        }   
        ammoDisplay.UpdatDisplay(weapon.Ammo(), weapon.Rounds());
    }

    private void OnHit(RaycastHit hit)
    {
        //if hit something log it
        GameObject target = hit.collider.gameObject;
        switch (target.tag)
        {
            case "Enemy":
                Debug.Log("Hit enemy");
                target.gameObject.GetComponent<EnemyBase>().TakeDamage((int)weapon.Damage());
                break;
            default:
                Debug.Log("Hit " + target.name);
                break;
        }
    }
}
