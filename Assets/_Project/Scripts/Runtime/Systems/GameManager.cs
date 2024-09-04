using UnityEngine;


public enum EnemyState
{
    Idle, Patrol, Follow, Alert, Combat, Run
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
    public int percPatrol;

    public bool RandomSystem(int perc)
    {
        int id = Random.Range(0, 100);
        bool ret = id <= perc ? true : false;
        return ret;
    }
}
