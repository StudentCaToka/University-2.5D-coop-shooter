using UnityEngine;

public class EnemyDefendSpot : IFSMState<EnemyController>
{
    private Vector3 _directionToSpot;

    static readonly EnemyDefendSpot instance = new EnemyDefendSpot();
    public static EnemyDefendSpot Instance
    {
        get { return instance; }
    }

    public void Enter(EnemyController e)
    {

    }

    public void Exit(EnemyController e)
    {

    }

    public void Reason(EnemyController e)
    {
        
    }

    public void Update(EnemyController e)
    {
        _directionToSpot = e.spotTransform.position - e.transform.position;

        DetectPlayers(e);

        if (e.closestPlayer != null)
        {
            e.transform.LookAt(e.closestPlayer.transform.position);
            e.agent.SetDestination(e.closestPlayer.transform.position);
        }

        if (Vector3.Magnitude(_directionToSpot) > 3.5f)
        {
            e.ChangeState(EnemyCaptureSpot.Instance);
        }
    }

    public void DetectPlayers(EnemyController e)
    {
        foreach (var player in e.connnectedPlayers)
        {
            if (Vector3.Magnitude(e.transform.position - player.transform.position) <= e.maxSpottingDistance)
            {
                if (e.closestPlayer == null)
                {
                    e.closestPlayer = player;
                }
                else if (Vector3.Magnitude(e.closestPlayer.transform.position) > Vector3.Magnitude(player.transform.position))
                {
                    e.closestPlayer = player;
                }
            }
        }
    }
}
