using UnityEngine;

public class EnemyCaptureSpot : IFSMState<EnemyController>
{
    private Vector3 _directionToSpot;
    private Transform _currentTarget;

    static readonly EnemyCaptureSpot instance = new EnemyCaptureSpot();
    public static EnemyCaptureSpot Instance
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

        if (_currentTarget == null)
        {
            return;
        }

        if (Vector3.Magnitude(_directionToSpot) < 3.5f)
        {
            e.agent.SetDestination(e.transform.position);

            e.animationController.SetBool("isMoving", false);

            e.ChangeState(EnemyDefendSpot.Instance);

            return;
        }

        e.transform.LookAt(_currentTarget);
        e.agent.SetDestination(_currentTarget.position);

        e.animationController.SetBool("isMoving", true);
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

                _currentTarget = e.closestPlayer.transform;
            }
            else
            {
                _currentTarget = e.spotTransform;
            }
        }
    }
}
