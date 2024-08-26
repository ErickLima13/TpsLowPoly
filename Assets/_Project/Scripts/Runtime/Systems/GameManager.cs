using UnityEngine;


public enum EnemyState
{
    Idle, Patrol, Follow, Alert, Attack
}

public class GameManager : MonoBehaviour
{
    [Header("Enemy AI Setup")]
    public float idleWaitTime;
    public float patrolWaitTime;
    public int percPatrol;


    public bool RandomSystem(int perc)
    {
        int id = Random.Range(0, 100);
        bool ret = id <= perc ? true : false;
        return ret;
    }
}
