using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GameEvents;

public class Weapon : NetworkBehaviour {

    [SerializeField]
    private Weapons currentWeapon;

	void Start () {
		
	}
	
	void Update () {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            GameEventManager.TriggerEvent(new Inventory_GameEvent(InventoryEventType.SwitchWeapon, Weapons.AssaultRifle));
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            GameEventManager.TriggerEvent(new Inventory_GameEvent(InventoryEventType.SwitchWeapon, Weapons.HandCannon));
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            GameEventManager.TriggerEvent(new Inventory_GameEvent(InventoryEventType.SwitchWeapon, Weapons.Railgun));
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            GameEventManager.TriggerEvent(new Inventory_GameEvent(InventoryEventType.SwitchWeapon, Weapons.RocketLauncher));
        }
    }
}
