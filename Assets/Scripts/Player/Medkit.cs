using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Medkit : MonoBehaviour {

    [SerializeField]
    private float healAmount;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered");

        if (other.transform.tag == "Player")
        {
            if (NetworkServer.active)
            {
                IDamageable damageable = other.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    damageable.Damage(healAmount * -1);

                    Destroy(this.gameObject);
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
