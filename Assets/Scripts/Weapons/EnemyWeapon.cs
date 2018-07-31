using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GameEvents;

public class EnemyWeapon : MonoBehaviour {

    [SerializeField]
    private float damageAmountPerSecond;
    [SerializeField]
    private Animator animationController;
    
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            if (NetworkServer.active)
            {
                IDamageable damageable = other.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    animationController.SetBool("attacking", true);

                    damageable.Damage(damageAmountPerSecond * Time.deltaTime);

                    GameEventManager.TriggerEvent(new Score_GameEvent(ScoreEventType.AddScore, Team.Enemy, damageAmountPerSecond * 10 * Time.deltaTime));
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        animationController.SetBool("attacking", false);
    }
}
