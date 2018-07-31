using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Stats", menuName = "Stats/Player", order = 1)]
public class PlayerStats : ScriptableObject
{
    [Header("Maximum Values")]
    public float maximumHealth = 0;
    public float maximumShield = 0;

    [Header("Current Values")]
    public float health = 0;
    public float shield = 0;
}