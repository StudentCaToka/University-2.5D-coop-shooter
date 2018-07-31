using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SimpleProjectile : MonoBehaviour, IWeapon
{
    [SerializeField]
    private Rigidbody projectileRigidBody;
    [SerializeField]
    private float damageAmount;
    [SerializeField]
    private float speed;

    private PlayerController projectileInstigator;
    private Collider instigatorCollider;
    private ObjectPooler objectPooler;

    private void Start()
    {
        objectPooler = ObjectPooler.Instance;
    }

    public void InitProjectile(Vector3 position, Vector3 direction, PlayerController instigator)
    {
        direction.Normalize();

        transform.position = position;
        projectileRigidBody.velocity = direction * speed;
        projectileInstigator = instigator;

        instigatorCollider = instigator.GetComponentInChildren<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != instigatorCollider || other.transform.tag == "Objective")
        {
            if (NetworkServer.active)
            {
                IDamageable damageable = other.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    damageable.Damage(damageAmount);

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
