using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;

enum SpotState
{
    Neutral,
    Player,
    Enemy,
    Contested
}

public class Spot : MonoBehaviour {

    [SerializeField]
    private SpotState _spotState;
    private Team team;
    [SerializeField]
    private int enemyCount;
    [SerializeField]
    private int playerCount;
    [Header("Scores")]
    [SerializeField]
    private float scorePerMinute;
    [SerializeField]
    private float scoreForEnemy;

    private void Update()
    {
        if (enemyCount > 0 || playerCount > 0)
        {
            if (enemyCount > 0 && playerCount == 0 && _spotState != SpotState.Enemy)
            {
                _spotState = SpotState.Enemy;

                team = Team.Enemy;
            }
            else if (playerCount > 0 && enemyCount == 0 && _spotState != SpotState.Player)
            {
                _spotState = SpotState.Player;

                team = Team.Player;
            }
            else if (enemyCount > 0 && playerCount > 0 && _spotState != SpotState.Contested)
            {
                _spotState = SpotState.Contested;
            }
        }
        else
        {
            if (_spotState != SpotState.Neutral)
            {
                _spotState = SpotState.Neutral;
            }
        }

        if (_spotState != SpotState.Neutral && _spotState != SpotState.Contested)
        {
            GameEventManager.TriggerEvent(new Score_GameEvent(ScoreEventType.AddScore, team, 10 * playerCount * Time.deltaTime));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            playerCount++;
        }
        else if (other.transform.tag == "Enemy")
        {
            enemyCount++;

            GameEventManager.TriggerEvent(new Score_GameEvent(ScoreEventType.AddScore, Team.Enemy, scoreForEnemy));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            playerCount--;
        }
        else if (other.transform.tag == "Enemy")
        {
            enemyCount--;
        }
    }
}
