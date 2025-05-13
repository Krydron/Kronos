using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/Weapon")]
public class Weapon : ScriptableObject
{
    [SerializeField] float damage;
    [SerializeField] int ammo;
    [SerializeField] int maxAmmo;
    [SerializeField] int rounds;
    [SerializeField] int maxRounds;
    [SerializeField] bool inInventory;

    public float Damage() { return damage; }
    public int Ammo() { return ammo; }
    public int MaxAmmo() { return maxAmmo; }
    public int Rounds() { return rounds; }
    public float MaxRounds() { return maxRounds; }

    public bool InInventory() { return inInventory; }

    public void InInventory(bool value) { inInventory = value; }

    public void Rounds(int value) { rounds = value; }

    public void Ammo(int value) { ammo = value; }
}