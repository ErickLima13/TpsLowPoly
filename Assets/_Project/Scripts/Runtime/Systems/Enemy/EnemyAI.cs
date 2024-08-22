using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameManager gameManager; 
    private Animator animator;

    public EnemyState currentState;

    [Header("Patrol Waypoints")]
    public int idWaypoint;
    public Transform[] waypoints;
    private Transform target;
    public bool isWaitWayPoint;
    private bool isEndPatrol;
    private bool isWaitPatrol;

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
                print("Entrou em Idle");
                break;
            case EnemyState.Patrol:
                isEndPatrol = false;
                idWaypoint = 1;
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
        animator.SetFloat("velocity", agent.desiredVelocity.magnitude);

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
            && !isEndPatrol && !isWaitPatrol)
        {
            if (isWaitWayPoint)
            {
                StartCoroutine("Patrol");
            }
            else
            {
                SetNewDestination();
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

    private IEnumerator Patrol()
    {
        isWaitPatrol = true;
        yield return new WaitForSeconds(gameManager.patrolWaitTime);
        SetNewDestination();
        isWaitPatrol = false;
    }

    private void SetNewDestination()
    {
        idWaypoint = (idWaypoint + 1) % waypoints.Length;
        SetDestinationAgent(waypoints[idWaypoint].position);
  
        if (idWaypoint.Equals(0))
        {
            agent.stoppingDistance = 0;
            isEndPatrol = true;
        }
    }


    #endregion

}
