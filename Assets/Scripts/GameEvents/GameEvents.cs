using UnityEngine;
using System.Collections;
using UnityEngine.Analytics;
using UnityEngine.Experimental.UIElements;


namespace GameEvents
{
    /// <summary>
    ///	base class for a game action / statechange / user input
    /// The event holds all data necessary to describe itself, so that listeners 
    /// are able to interpret the content.
    /// </summary>
    public abstract class GameEvent
    {
        static int _idSequencer = 0;
        public static int GetNextEventID() { return ++_idSequencer; }

        //	constructor
        public GameEvent()
        {
            uniqueEventID = GetNextEventID();
            time = Time.time;
        }

        /// <summary>
        /// it's always a good idea to have a unique identifier for collected information objects.
        /// </summary>
        public readonly int uniqueEventID;

        /// <summary>
        /// time the event occured
        /// </summary>
        public readonly float time;

        /// <summary>
        /// each event will be able to test its data.
        /// this way, it isn't important if a sounddesigner or programmer implement/change an event,
        /// as both will be notified in case a misunderstanding / miscommunication occured.
        /// This is a convenient way to optimize maintainability, as the code will tell you what's wrong.
        /// </summary>
        public virtual bool isValid() { return true; }
    }

    //---------------------------------------------------------------------------------------------------

    // sample events without additional arguments

    public class GameEvent_Click : GameEvent { }
    public class GameEvent_CancelAction : GameEvent { }
    public class GameEvent_FinishLevel : GameEvent { }

    //---------------------------------------------------------------------------------------------------

    // sample events with additional arguments. Make sure these implement Validate()

    public enum GameEngineEventType
    {
        LevelStart,
        LevelComplete,
        LevelEnd,
        EnemyDied
    }

    public class GameEvent_Engine : GameEvent
    {
        public GameEngineEventType GameEngineEventType;

        public GameEvent_Engine(GameEngineEventType t)
        {
            GameEngineEventType = t;
        }

        public override bool isValid()
        {
            return GameEngineEventType != null;
        }
    }

    public enum LocalNetworkEventType
    {
        PlayerConnected,
        PlayerDisconnected
    }

    public class LocalNetwork_EventType : GameEvent
    {
        public LocalNetworkEventType LocalNetworkEventType;
        public GameObject Player;

        public LocalNetwork_EventType(LocalNetworkEventType t, GameObject p)
        {
            LocalNetworkEventType = t;
            Player = p;
        }

        public override bool isValid()
        {
            return LocalNetworkEventType != null;
        }
    }

    public enum CameraEventType
    {
        ChangeDistance,
        ChangeTarget
    }

    public class Camera_GameEvent : GameEvent
    {
        public CameraEventType CameraEventType;
        public float CameraDistance;
        public Transform CameraTarget;

        public Camera_GameEvent(CameraEventType t, float d)
        {
            CameraEventType = t;
            CameraDistance = d;
        }

        public Camera_GameEvent(CameraEventType t, Transform p)
        {
            CameraEventType = t;
            CameraTarget = p;
        }

        public override bool isValid()
        {
            return CameraEventType != null;
        }
    }

    public enum InventoryEventType
    {
        SwitchWeapon
    }

    public class Inventory_GameEvent : GameEvent
    {
        public InventoryEventType InventoryEventType;
        public Weapons Weapon;

        public Inventory_GameEvent(InventoryEventType t, Weapons w)
        {
            InventoryEventType = t;
            Weapon = w;
        }

        public override bool isValid()
        {
            return InventoryEventType != null;
        }
    }

    public enum ScoreEventType
    {
        AddScore,
        RemoveScore
    }

    public class Score_GameEvent : GameEvent
    {
        public ScoreEventType ScoreEventType;
        public Team Team;
        public float Score;

        public Score_GameEvent(ScoreEventType t, Team w, float s)
        {
            ScoreEventType = t;
            Team = w;
            Score = s;
        }

        public override bool isValid()
        {
            return ScoreEventType != null;
        }
    }
}