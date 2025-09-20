using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]
public class PlayerData : ScriptableObject
{
    [Header("Move State")]
    public float movementVelocity = 10f;
    
    [Header("Dash State")]
    public float dashCooldown = 0.2f;
    public float dashVelocity = 30f;
    public float dashDuration = 0.2f;
    public float dashDrag = 1f;
    
    [Header("Attack State")]
    public float attackDrag = 10f;
}