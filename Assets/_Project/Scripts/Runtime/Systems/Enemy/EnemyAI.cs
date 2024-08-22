using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    // Auto battler dos soldadinhos, eles se atacam e esperam x tempo, O modificador externo vai ser rolar um dado de 4 faces, 
    // 1 - aumenta a velocidade de ataque. 2 - aumenta o poder de ataque, 3 - aumenta a vida, 4 - ganha outro dado.

    private NavMeshAgent agent;
    private GameManager gameManager; 

    public EnemyState currentState;

    [Header("Patrol Waypoints")]
    public int idWaypoint;
    public Transform[] waypoints;
    private Transform target;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        agent = GetComponent<NavMeshAgent>();
        SetDestinationAgent(transform.position);
        OnStateEnter(currentState);
    }

    private void Update()
    {
        UpdateEnemyState();
    }

    private void SetDestinationAgent(Vector3 destination)
    {
        agent.destination = destination;
    }

    #region State Machine

    private void OnStateEnter(EnemyState newState)
    {
        StopAllCoroutines();
        currentState = newState;

        switch (currentState)
        {
            case EnemyState.Idle:
                StartCoroutine("Idle");
                print("Entrou em Idle");
                break;
            case EnemyState.Patrol:
               

                print("Entrou em Patrol");
                break;
            case EnemyState.Follow:
                break;
            case EnemyState.Alert:
                break;
            case EnemyState.Attack:
                break;
        }
    }

    private void UpdateEnemyState()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patroling();
                break;
            case EnemyState.Follow:
                break; 
        }
    }

    private void Patroling()
    {

    }

    private IEnumerator Idle()
    {
        SetDestinationAgent(transform.position);
        yield return new WaitForSeconds(gameManager.idleWaitTime);

        if (gameManager.RandomSystem(gameManager.percPatrol))
        {
            OnStateEnter(EnemyState.Patrol);
        }
        else
        {
            OnStateEnter(EnemyState.Idle);
        }
    }

  

    #endregion

}
