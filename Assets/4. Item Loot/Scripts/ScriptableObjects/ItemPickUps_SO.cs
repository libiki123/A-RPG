using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemTypeDefinitions { HEALTH, WEALTH, MANA, WEAPON, ARMOR, BUFF, EMPTY };
public enum ItemArmorSubType { None, Head, Chest, Hands, Legs, Boots };

[CreateAssetMenu(fileName = "NewItem", menuName = "Spawnable Item/New Pick-up", order = 1)]
public class ItemPickUps_SO : ScriptableObject
{
    public string itemName = "New Item";
    public ItemTypeDefinitions itemType = ItemTypeDefinitions.HEALTH;
    public ItemArmorSubType itemArmorSubType = ItemArmorSubType.None;
    public int itemAmount = 0;                          // Common for wealth/weapon/amor, mostly for currency 
    public int spawnChanceWeight = 0;                   // possibility of spawning item

    public Material itemMaterial = null;
    public Sprite itemIcon = null;
    public Rigidbody itemSpawnObject = null;            // Object gonna use within gameplay
    public Weapon weaponSlotObject = null;              // Object that will spwan out of chests

    public bool isEquipped = false;
    public bool isInteractable = false;             // For item that player can use (NOT ex: wealth, currency, mana.....)
    public bool isStorable = false;                 // Can/can't be storage in the storage 
    public bool isUnique = false;                   // Is it commonly clone item?
    public bool isIndestructable = false;           // Usually for weapons (with durability ele)
    public bool isQuestItem = false;                // Is the item relate to the quest lines (usually toggle with isUnique)
    public bool isStackable = false;
    public bool destroyOnUse = false;                   // Will it be destoyed on use
    public float itemWeight = 0f;
}
