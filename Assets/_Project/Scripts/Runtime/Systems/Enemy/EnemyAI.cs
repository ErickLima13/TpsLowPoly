using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    // Auto battler dos soldadinhos, eles se atacam e esperam x tempo, O modificador externo vai ser rolar um dado de 4 faces, 
    // 1 - aumenta a velocidade de ataque. 2 - aumenta o poder de ataque, 3 - aumenta a vida, 4 - ganha outro dado.

    private NavMeshAgent agent;
    private GameManager gameManager; 
    private Animator animator;

    public EnemyState currentState;

    [Header("Patrol Waypoints")]
    public int idWaypoint;
    public Transform[] waypoints;
    private Transform target;
    private bool isEndPatrol;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();
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
                animator.SetInteger("IdAnimation", 0);
                print("Entrou em Idle");
                break;
            case EnemyState.Patrol:
                animator.SetInteger("IdAnimation", 1);
                isEndPatrol = false;
                idWaypoint = 0;
                SetDestinationAgent(waypoints[idWaypoint].position);
                agent.stoppingDistance = 0.5f;
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
        if (agent.remainingDistance <= agent.stoppingDistance
            && !isEndPatrol)
        {
            idWaypoint = (idWaypoint + 1) % waypoints.Length;
            SetDestinationAgent(waypoints[idWaypoint].position);

            if (idWaypoint.Equals(0))
            {
                agent.stoppingDistance = 0;
                isEndPatrol = true;
            }
        }
        else if (agent.remainingDistance <= agent.stoppingDistance
            && isEndPatrol)
        {
            OnStateEnter(EnemyState.Idle);
        }
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
