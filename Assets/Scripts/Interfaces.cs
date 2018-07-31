using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IDamageable
{
    void Damage(float damageAmount);
}

public interface IWeapon
{
    void InitProjectile(Vector3 position, Vector3 direction, PlayerController instigator);
}