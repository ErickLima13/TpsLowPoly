using UnityEngine;


public enum EnemyType
{
    Patrol, Guard
}

public enum EnemyState
{
    Idle, Patrol, Follow, Alert, Combat, Run, Safe, Die
}

public class GameManager : MonoBehaviour
{
    [Header("Enemy AI Setup")]
    public float idleWaitTime;
    public float patrolWaitTime;
    public float alertWaitTime;
    public float aimDuration = 0.2f;
    public float timeToCheck;
    public float distanceToAttack;
    public float combatTime;
    public float rotationSpeed;
    public float viewRadiusBase;
    public int percPatrol;

    public bool RandomSystem(int perc)
    {
        int id = Random.Range(0, 100);
        bool ret = id <= perc ? true : false;
        return ret;
    }
}
