using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameManager gameManager;
    private Animator animator;
    private FieldOfView fov;
    private RaycastWeapon raycastWeapon;
    private AnimationControl controlWeapon;

    private bool isEndPatrol;
    private bool isWaitPatrol;
    private bool isAim;

    private float attackTime;
    private float distancePlayer;

    public EnemyState currentState;

    [Header("Patrol Waypoints")]
    public int idWaypoint;
    public Transform[] waypoints;
    private Transform target;
    private Vector3 lookInto;
    public bool isWaitWayPoint;

    [Header("Rig")]
    public Rig weaponAim;

    [Header("Id Weapon")]
    public Transform[] originRaycast;
    public Weapon[] weapons;


    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        fov = GetComponentInChildren<FieldOfView>();
        raycastWeapon = GetComponentInChildren<RaycastWeapon>();
        controlWeapon = GetComponentInChildren<AnimationControl>();

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        SetDestinationAgent(transform.position);
        OnStateEnter(currentState);

        foreach (Weapon weapon in weapons)
        {
            weapon.StartWeapon();
        }
    }

    private void Update()
    {
        UpdateEnemyState();

        if (isAim)
        {
            weaponAim.weight +=
                Time.deltaTime / gameManager.aimDuration;
        }
        else
        {
            weaponAim.weight -=
           Time.deltaTime / gameManager.aimDuration;
        }
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
                break;
            case EnemyState.Patrol:
                SetDestinationAgent(
                    waypoints[idWaypoint].position);
                agent.stoppingDistance = 0.5f;
                isAim = false;
                break;
            case EnemyState.Follow:
                break;
            case EnemyState.Alert:
                target = fov.visibleSecondary[0];
                lookInto = target.position;
                SetDestinationAgent(transform.position);
                StartCoroutine("Alert");
                break;
            case EnemyState.Combat:
                agent.stoppingDistance = 9;
                target = fov.visibleTargets[0];
                raycastWeapon.raycastDestination = target;
                StartCoroutine("Combat");
                break;
        }

        print("Entrou em " + currentState);
    }

    private void UpdateEnemyState()
    {
        animator.SetFloat("velocity",
            agent.desiredVelocity.magnitude);

        switch (currentState)
        {
            case EnemyState.Patrol:
                Patroling();
                break;
            case EnemyState.Follow:
                break;
            case EnemyState.Combat:
                isAim = true;
                SetDestinationAgent(target.position);
                transform.LookAt(target);
                distancePlayer = Vector3.Distance(
                    transform.position, target.position);

                if (distancePlayer <= gameManager.distanceToAttack)
                {
                    attackTime = 0;
                }
                else
                {
                    attackTime += Time.deltaTime;
                    if (attackTime >= gameManager.combatTime)
                    {
                        OnStateEnter(EnemyState.Idle);
                    }
                }

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

    #region Corrotines

    private IEnumerator Idle()
    {
        SetDestinationAgent(transform.position);
        yield return new WaitForSeconds(
            gameManager.idleWaitTime);

        if (gameManager.RandomSystem(gameManager.percPatrol))
        {
            isEndPatrol = false;
            idWaypoint = 1;
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
        yield return new WaitForSeconds(
            gameManager.patrolWaitTime);
        SetNewDestination();
        isWaitPatrol = false;
    }

    private IEnumerator Alert()
    {
        yield return new WaitForSeconds(
            gameManager.alertWaitTime);
        isAim = true;
        agent.stoppingDistance = 3;
        SetDestinationAgent(lookInto);
        yield return new WaitUntil(() =>
        agent.remainingDistance <= agent.stoppingDistance);

        if (fov.visibleSecondary.Count > 0)
        {
            yield return new WaitForSeconds(0.3f);
            OnStateEnter(EnemyState.Combat);
        }
        else
        {
            yield return new WaitForSeconds(
                gameManager.patrolWaitTime);
            agent.stoppingDistance = 1;
            yield return new WaitUntil(() =>
            agent.remainingDistance <= agent.stoppingDistance);
            yield return new WaitForSeconds(
                gameManager.patrolWaitTime);
            OnStateEnter(EnemyState.Patrol);
        }
    }

    private IEnumerator Combat()
    {
        while (true)
        {
            if (distancePlayer <= gameManager.distanceToAttack)
            {
                for (int b = 0; b < weapons[controlWeapon.idWeapon].blastShots; b++)
                {
                    if (weapons[controlWeapon.idWeapon].ammunition > 0)
                    {
                        weapons[controlWeapon.idWeapon].ammunition--;
                        raycastWeapon.StartFire(
                        originRaycast[controlWeapon.idWeapon],
                        weapons[controlWeapon.idWeapon].weaponDamage);
                        yield return new WaitForSeconds(weapons[controlWeapon.idWeapon].delayBetweenBullets);
                    }
                    else
                    {
                        if (weapons[controlWeapon.idWeapon].ammunition <= 0)
                        {
                            if (weapons[controlWeapon.idWeapon].ammunitionExtra > 0)
                            {
                                yield return new WaitForSeconds(weapons[controlWeapon.idWeapon].timeToReaload);
                                weapons[controlWeapon.idWeapon].Reload();
                            }
                            else
                            {
                                OnStateEnter(EnemyState.Run);
                            }
                        }
                    }
                }

                yield return new WaitForSeconds(weapons[controlWeapon.idWeapon].delayBetweenShots);
            }
            else
            {
                yield return new WaitForSeconds(gameManager.timeToCheck);
            }
        }
    }

    #endregion

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

    private void IsVisible(FieldOfView.ViewState vState)
    {
        if (currentState == EnemyState.Combat
            || currentState == EnemyState.Run)
        {
            return;
        }

        switch (vState)
        {
            case FieldOfView.ViewState.Primary:
                if (currentState != EnemyState.Combat)
                {
                    OnStateEnter(EnemyState.Combat);
                }
                break;
            case FieldOfView.ViewState.Secondary:
                if (currentState != EnemyState.Alert
                    && currentState != EnemyState.Combat)
                {
                    OnStateEnter(EnemyState.Alert);
                }
                break;
        }
    }

    #endregion

    public bool Hit()
    {
        return gameManager.RandomSystem(weapons[controlWeapon.idWeapon].accuracy);
    }
}