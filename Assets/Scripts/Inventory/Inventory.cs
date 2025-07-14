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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable] public class Key
{
    public Doors.DoorType type;
    public bool have;
}
public class Inventory : MonoBehaviour
{
    [SerializeField] private int money;
    [SerializeField] public List<Key> keys;
    [SerializeField] public Weapon[] weapons;
    [SerializeField] public Weapon[] items;
    [SerializeField] private List<Item> collectedItems = new List<Item>();
    public uint weaponPointer;
    private CashDisplay cashDisplay;

    private AmmoDisplay ammoDisplay;

    [SerializeField] GameObject gunUI;

    public void ActivateKeyType(Doors.DoorType type)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            if (keys[i].type == type)
            {
                keys[i].have = true;
            }
        }
    }

    public void AddKeycardToInventory(Keycard keycard)
    {
        // Create a new Key object with the type of the keycard and set it to 'have'
        Key newKey = new Key { type = keycard.GetKeyType(), have = true };

        // Add the new key to the keys list in the inventory
        keys.Add(newKey);

        // Log the addition of the keycard to the inventory for debugging
        Debug.Log("Keycard " + keycard.GetKeyType() + " added to inventory.");
    }




    public void IncreaseMoney(int value)
    {
        money += value;
        cashDisplay.UpdateCashDisplay(money);
    }
    public void DecreaseMoney(int value)
    {
        money -= value;
        if (money < 0) { money = 0; }
        //cashDisplay.UpdateCashDisplay(money);
    }

    public void UpdateInventoryWeapons()
    {
        foreach (Weapon weapon in weapons)
        {
            if (weapon.InInventory())
            {
                //Update UI
            }
        }
    }

    public Weapon GetWeapon() { 
        if (weaponPointer > weapons.Length) {
            Debug.Log(weaponPointer + " " + weapons.Length);
            return null; 
        }
        return weapons[weaponPointer]; 
    }

    public void OnChangeWeapon1()
    {
        weaponPointer = 0;
        UpdateAmmoDisplay();
        gunUI.SetActive(true);
    }
    public void OnChangeWeapon2()
    {
        weaponPointer = 1;
        UpdateAmmoDisplay();
        gunUI.SetActive(false);
    }

    private void UpdateAmmoDisplay()
    {
        ammoDisplay = GameObject.Find("AmmoDisplay").GetComponent<AmmoDisplay>();
        ammoDisplay.UpdatDisplay(weapons[weaponPointer].Ammo(), weapons[weaponPointer].Rounds());
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //weapons = Resources.FindObjectsOfTypeAll<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            weapon.InInventory(true);
            weapon.ResetAmmo(); 
        }
        weaponPointer = 0;
        //money = 0;
        //ammoDisplay = GameObject.Find("AmmoDisplay").GetComponent<AmmoDisplay>();
        //cashDisplay = GameObject.Find("CashDisplay").GetComponent<CashDisplay>();
        //cashDisplay.UpdateCashDisplay(100);
        //GameObject.Find("CashDisplay").GetComponent<CashDisplay>().UpdateCashDisplay(0);
        //UpdateAmmoDisplay();
    }

    public void AddItemToInventory(Item item)
    {
        if (!collectedItems.Contains(item))
        {
            collectedItems.Add(item);
            Debug.Log($"Item '{item.itemName}' added to inventory.");
        }
    }

    public bool HasItem(Item item)
    {
        return collectedItems.Contains(item);
    }

    public void RemoveItem(Item item)
    {
        if (collectedItems.Contains(item))
        {
            collectedItems.Remove(item);
            Debug.Log($"Item '{item.itemName}' removed from inventory.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
