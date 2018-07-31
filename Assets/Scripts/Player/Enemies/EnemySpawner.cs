using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GameEvents;

public class EnemySpawner : NetworkBehaviour,
    IGameEventListener<LocalNetwork_EventType>
{

    [SerializeField]
    private GameObject enemyPrefab;

    private Vector3 _spawnPosition;
    private Quaternion _spawnRotation;

    private List<GameObject> connnectedPlayers = new List<GameObject>();

    public void Awake()
    {
        _spawnPosition = this.transform.position;
        _spawnRotation = this.transform.rotation;
    }

    public override void OnStartServer()
    {
        CmdSpawnEnemy();
    }

    [Command]
    public void CmdSpawnEnemy()
    {
        GameObject newEnemy = Instantiate(enemyPrefab, _spawnPosition, _spawnRotation, this.transform);
        newEnemy.GetComponent<EnemyController>().connnectedPlayers = connnectedPlayers;

        NetworkServer.Spawn(newEnemy);
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
