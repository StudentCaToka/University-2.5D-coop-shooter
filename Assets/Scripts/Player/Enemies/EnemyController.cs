using UnityEngine;
using System.Collections.Generic;
using GameEvents;
using UnityEngine.AI;
using UnityEngine.Networking;

public class EnemyController : StatefulMonoBehaviour<EnemyController>,
    IGameEventListener<LocalNetwork_EventType>
{
    public float speed;
    public int maxSpottingDistance;

    public GameObject closestPlayer;

    public Transform playerTransform;
    public Transform spotTransform;

    public NavMeshAgent agent;
    public Animator animationController;
    public Rigidbody localRigidbody;

    public List<GameObject> connnectedPlayers = new List<GameObject>();

    #region OnEnable + OnDisable

    private void OnEnable()
    {
        this.EventStartListening<LocalNetwork_EventType>();
    }

    private void OnDisable()
    {
        this.EventStopListening<LocalNetwork_EventType>();
    }

    #endregion

    void Awake()
    {
        fsm = new FSM<EnemyController>();
        fsm.Configure(this, new EnemyCaptureSpot());

        spotTransform = GameObject.FindGameObjectWithTag("Objective").transform;

        localRigidbody = this.GetComponent<Rigidbody>();
    }

    public void OnGameEvent(LocalNetwork_EventType eventType)
    {
        switch (eventType.LocalNetworkEventType)
        {
            case LocalNetworkEventType.PlayerConnected:

                connnectedPlayers.Add(eventType.Player);

                Debug.Log("Player connected");

                break;

            case LocalNetworkEventType.PlayerDisconnected:

                connnectedPlayers.Remove(eventType.Player);

                break;

            default:

                break;
        }
    }
}
