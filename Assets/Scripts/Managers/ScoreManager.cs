using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Team
{
    Player,
    Enemy
}

public class ScoreManager : NetworkBehaviour,
    IGameEventListener<Score_GameEvent>
{
    [Header("ScoreCounters")]
    [SerializeField]
    private Text scorePlayersText;
    [SerializeField]
    private Text scoreEnemiesText;

    [SyncVar]
    public float scorePlayers;
    [SyncVar]
    public float scoreEnemies;

    [SerializeField]
    private int scorePlayersInt;
    [SerializeField]
    private int scoreEnemiesInt;

    #region OnEnable + OnDisable

    private void OnEnable()
    {
        this.EventStartListening<Score_GameEvent>();
    }

    private void OnDisable()
    {
        this.EventStopListening<Score_GameEvent>();
    }

    #endregion

    private void FixedUpdate()
    {
        scorePlayersInt = Mathf.FloorToInt(scorePlayers);
        scoreEnemiesInt = Mathf.FloorToInt(scoreEnemies);

        scorePlayersText.text = scorePlayersInt.ToString("D6");
        scoreEnemiesText.text = scoreEnemiesInt.ToString("D6");
    }

    private void IncreaseScore(Team team, float score)
    {
        if (NetworkServer.active)
        {
            switch (team)
            {
                case Team.Player:

                    scorePlayers += score;

                    break;

                case Team.Enemy:

                    scoreEnemies += score;

                    break;

                default:

                    break;
            }

            if (scorePlayers >= 15000)
            {
                NetworkServer.DisconnectAll();

                SceneManager.LoadScene(2);

                //CmdSwitchWinningScreen();
            }
            else if (scoreEnemies >= 10000)
            {
                NetworkServer.DisconnectAll();

                SceneManager.LoadScene(3);

                //CmdSwitchLosingScreen();
            }
        }
    }

    private void DecreaseScore(Team team, float score)
    {
        if (NetworkServer.active)
        {
            switch (team)
            {
                case Team.Player:

                    scorePlayers -= score;

                    scorePlayersInt = Mathf.FloorToInt(scorePlayers);

                    scorePlayersText.text = scorePlayersInt.ToString("D6");

                    break;

                case Team.Enemy:

                    scoreEnemies -= score;

                    scoreEnemiesInt = Mathf.FloorToInt(scoreEnemies);

                    scoreEnemiesText.text = scoreEnemiesInt.ToString("D6");

                    break;

                default:

                    break;
            }
        }
    }

    //apparently this doesnt work in any way i tried it so only the host will now see the winning screen
    [Command]
    private void CmdSwitchWinningScreen()
    {
        NetworkManager.singleton.autoCreatePlayer = false;
        NetworkManager.singleton.ServerChangeScene("EndScreenWon");
    }

    [Command]
    private void CmdSwitchLosingScreen()
    {
        NetworkManager.singleton.autoCreatePlayer = false;
        NetworkManager.singleton.ServerChangeScene("EndScreenLost");
    }

    public void OnGameEvent(Score_GameEvent eventType)
    {
        switch (eventType.ScoreEventType)
        {
            case ScoreEventType.AddScore:

                IncreaseScore(eventType.Team, eventType.Score);
                break;

            case ScoreEventType.RemoveScore:
                DecreaseScore(eventType.Team, eventType.Score);
                break;

            default:
                break;
        }
    }
}
