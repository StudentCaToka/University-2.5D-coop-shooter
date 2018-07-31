using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Networking;
using System.Collections;
using GameEvents;

public class Health : NetworkBehaviour, IDamageable
{
    [SerializeField]
    private float maxHealth;

    [Header("Teams")]
    [SerializeField]
    private Team friendlyTeam;
    [SerializeField]
    private Team enemyTeam;

    public bool isEnemy;

    [SyncVar(hook = "OnChangeHealth")]
    private float currentHealth;

    [SerializeField]
    private RectTransform healthBar;
    private float healthBarSizeX = 1;

    private NetworkStartPosition[] spawnPoints;

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
    }

    public override void OnStartLocalPlayer()
    {
        if (isLocalPlayer)
        {
            healthBar = GameObject.Find("PlayerHUD/PlayerInfo/PlayerIcon/HP_back/HP_front").GetComponent<RectTransform>();

            spawnPoints = FindObjectsOfType<NetworkStartPosition>();

            healthBarSizeX = healthBar.sizeDelta.x / 100;
        }
    }

    public void Damage(float amount)
    {
        if (!isServer)
        {
            return;
        }

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            GameEventManager.TriggerEvent(new Score_GameEvent(ScoreEventType.AddScore, enemyTeam, 100));

            if (isEnemy)
            {
                StartCoroutine(KillEnemy());
            }
            else
            {
                RpcRespawn();
            }

            currentHealth = maxHealth;
        }
    }

    void OnChangeHealth(float currentHealth)
    {
        if (healthBar == null)
        {
            return;
        }

        healthBar.sizeDelta = new Vector2(currentHealth * healthBarSizeX, healthBar.sizeDelta.y);
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            Vector3 spawnPoint = Vector3.zero;

            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }

            transform.position = spawnPoint;
        }
    }

    IEnumerator KillEnemy()
    {
        this.GetComponent<NavMeshAgent>().enabled = false;
        this.GetComponent<EnemyController>().enabled = false;

        this.transform.localPosition += Vector3.down * 10;

        yield return new WaitForSeconds(1f);

        this.transform.parent.GetComponent<EnemySpawner>().CmdSpawnEnemy();

        Destroy(this.gameObject);
    }
}
