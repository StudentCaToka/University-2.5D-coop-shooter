using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using GameEvents;

public enum Weapons
{
    AssaultRifle,
    HandCannon,
    Railgun,    
    RocketLauncher,
}

public class InventoryManager : NetworkBehaviour, IGameEventListener<Inventory_GameEvent>
{
    [SerializeField]
    private Weapons currentWeapon;

    private void OnEnable()
    {
        this.EventStartListening<Inventory_GameEvent>();
    }

    private void OnDisable()
    {
        this.EventStopListening<Inventory_GameEvent>();
    }

    #region GameEvent

    public void OnGameEvent(Inventory_GameEvent eventType)
    {
        switch (eventType.InventoryEventType)
        {
            case InventoryEventType.SwitchWeapon:
                currentWeapon = eventType.Weapon;
                break;

            default:
                break;
        }
    }

    #endregion
}
