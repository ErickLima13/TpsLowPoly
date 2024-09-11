using System.Collections;
using System.Linq.Expressions;
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
    private RagDollColliders ragDoll;

    private int idWeapon;

    private bool isEndPatrol;
    private bool isWaitPatrol;
    private bool isAim;
    private bool damaged;
    private bool isAttackPosition;

    private float attackTime;
    private float distancePlayer;

    public EnemyType enemyType;
    public EnemyState currentState;

    [Header("Patrol Waypoints")]
    public Transform[] waypoints;
    public bool isWaitWayPoint;
    public int idWaypoint;
    private Transform target;
    private Vector3 lookInto;

    [Header("Safe Points")]
    public Transform[] safePoints;
    private int idSafePoint;

    [Header("Attack Points")]
    public Transform[] attackPoints;
    public int idAttackPoint;

    [Header("Rig")]
    public Rig weaponAim;
    public Rig handPose;

    [Header("Id Weapon")]
    public Transform[] originRaycast;
    public Weapon[] weapons;



    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        fov = GetComponentInChildren<FieldOfView>();
        raycastWeapon = GetComponentInChildren<RaycastWeapon>();
        controlWeapon = GetComponentInChildren<AnimationControl>();
        ragDoll = GetComponentInChildren<RagDollColliders>();
        idWeapon = controlWeapon.idWeapon;
        target = GameObject.FindWithTag("Player").transform;
        raycastWeapon.raycastDestination = target;

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
                agent.speed = 1.8f;
                //fov.viewRadius = gameManager.viewRadiusBase;  
                damaged = false;
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
                lookInto = target.position;
                SetDestinationAgent(transform.position);
                StartCoroutine("Alert");
                break;
            case EnemyState.Combat:
                agent.speed = 3.6f;
                switch (enemyType)
                {
                    case EnemyType.Patrol:
                        animator.SetBool("isCrouching", false);
                        LookAtTarget();
                        fov.viewRadius = 20;
                        agent.stoppingDistance = 7;
                        StartCoroutine("Combat");
                        break;
                    case EnemyType.Guard:
                        // ir ate o ponto de ataque
                        isAttackPosition = false;
                        agent.stoppingDistance = 0f;
                        SetDestinationAgent(
                            attackPoints[idAttackPoint].position);
                        break;
                }
                break;
            case EnemyState.Run:
                StartCoroutine("CheckSafePoint");
                break;
            case EnemyState.Die:
                isAim = false;
                animator.SetBool("isDie", true);
                agent.enabled = false;
                handPose.weight = 0;
                GetComponent<Collider>().enabled = false;
                controlWeapon.DisableAnimator();
                originRaycast[idWeapon].parent.gameObject.SetActive(false);              
                StartCoroutine("DieAnimation");
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
                switch (enemyType)
                {
                    case EnemyType.Patrol:
                        isAim = true;
                        SetDestinationAgent(target.position);
                        LookAtTarget();
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
                    case EnemyType.Guard:
                        if (InDestiny() && !isAttackPosition)
                        {
                            isAttackPosition = true;
                            SetDestinationAgent(transform.position);
                            StartCoroutine("CombatGuard");
                            animator.SetBool("isCrouching", true);
                            isAim = false;
                        }
                        else if (isAttackPosition)
                        {
                            LookAtTarget();
                        }


                        break;
                }




                break;
            case EnemyState.Run:
                if (InDestiny())
                {
                    animator.SetBool("isCrouching", true);
                    StartCoroutine("WaitInSafeArea");
                }

                break;
        }
    }

    private void Patroling()
    {
        if (InDestiny()
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
        else if (InDestiny()
            && isEndPatrol)
        {
            OnStateEnter(EnemyState.Idle);
        }
    }

    #region Corrotines

    private IEnumerator DieAnimation()
    {
        ragDoll.ActiveRagDoll();
        yield return new WaitForSeconds(1f);
        animator.enabled = false;
    }

    private IEnumerator CombatGuard()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.7f,1.5f));
            isAim = true;
            animator.SetBool("isCrouching", false);

            yield return new WaitForSeconds(0.5f);

            for (int b = 0; b < weapons[idWeapon].blastShots; b++)
            {
                raycastWeapon.StartFire(
                originRaycast[idWeapon],
                weapons[idWeapon].weaponDamage);
                yield return new WaitForSeconds(weapons[idWeapon].delayBetweenBullets);
            }

            yield return new WaitForSeconds(0.3f);

            isAim = false;
            animator.SetBool("isCrouching", true);
            yield return new WaitForSeconds(2f);

            if (attackPoints.Length > 0)
            {
                if (gameManager.RandomSystem(100))
                {
                    animator.SetBool("isCrouching", false);
                    yield return new WaitForSeconds(0.3f);
                    idAttackPoint++;
                    if (idAttackPoint >= attackPoints.Length)
                    {
                        idAttackPoint = 0;
                    }

                    OnStateEnter(EnemyState.Combat);
                }
            }
        }
    }

    private IEnumerator Idle()
    {
        SetDestinationAgent(transform.position);
        yield return new WaitForSeconds(
            gameManager.idleWaitTime);

        switch (enemyType)
        {
            case EnemyType.Patrol:
                if (gameManager.RandomSystem(gameManager.percPatrol))
                {
                    isEndPatrol = false;
                    //idWaypoint = 1;
                    OnStateEnter(EnemyState.Patrol);
                }
                else
                {
                    OnStateEnter(EnemyState.Idle);
                }
                break;
            case EnemyType.Guard:

                break;
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
        if (!damaged)
        {
            yield return new WaitForSeconds(
        gameManager.alertWaitTime);
        }

        isAim = true;
        agent.stoppingDistance = 3;
        SetDestinationAgent(lookInto);
        yield return new WaitUntil(() => InDestiny());

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
            yield return new WaitUntil(() => InDestiny());
            yield return new WaitForSeconds(
                gameManager.patrolWaitTime);
            OnStateEnter(EnemyState.Patrol);
        }
    }

    private IEnumerator Combat()
    {
        while (true)
        {
            if (distancePlayer <= gameManager.distanceToAttack &&
                fov.visibleTargets.Count > 0)
            {
                for (int b = 0; b < weapons[idWeapon].blastShots; b++)
                {
                    if (weapons[idWeapon].ammunition > 0)
                    {
                        weapons[idWeapon].ammunition--;
                        raycastWeapon.StartFire(
                        originRaycast[idWeapon],
                        weapons[idWeapon].weaponDamage);
                        yield return new WaitForSeconds(weapons[idWeapon].delayBetweenBullets);
                    }
                    else
                    {
                        if (weapons[idWeapon].ammunition <= 0)
                        {

                            OnStateEnter(EnemyState.Run);
                            print("vou carregar");

                            if (weapons[idWeapon].ammunitionExtra > 0)
                            {


                            }
                            else
                            {
                                // caso a munição acabe
                            }
                        }
                    }
                }

                yield return new WaitForSeconds(weapons[idWeapon].delayBetweenShots);
            }
            else
            {
                yield return new WaitForSeconds(gameManager.timeToCheck);
            }
        }
    }

    private IEnumerator CheckSafePoint()
    {
        isAim = false;
        bool isSafe = false;

        while (!isSafe)
        {
            idSafePoint = Random.Range(0, safePoints.Length);

            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 toOther = safePoints[idSafePoint].position - transform.position;

            if (Vector3.Dot(forward, toOther) < 0)
            {
                agent.stoppingDistance = 0f;
                agent.SetDestination(safePoints[idSafePoint].position);
                isSafe = true;
            }

            yield return new WaitForEndOfFrame();
        }

        //if (isSafe && weapons[idWeapon].ammunitionExtra > 0)
        //{
        //    yield return new WaitForSeconds(weapons[idWeapon].timeToReaload);
        //    weapons[idWeapon].Reload();
        //    print("Carreguei");
        //}
    }

    private IEnumerator WaitInSafeArea()
    {
        yield return new WaitForSeconds(weapons[idWeapon].timeToReaload);
        weapons[idWeapon].Reload();
        print("Carreguei");
        OnStateEnter(EnemyState.Combat);
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
        if (currentState == EnemyState.Die)
        {
            return;
        }

        if (currentState == EnemyState.Safe)
        {
            animator.SetBool("isCrouching", false);
            OnStateEnter(EnemyState.Run);
            return;
        }

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
        return gameManager.RandomSystem(weapons[idWeapon].accuracy);
    }

    public bool InDestiny()
    {
        return agent.remainingDistance <= agent.stoppingDistance;
    }

    private void Die()
    {
        OnStateEnter(EnemyState.Die);
    }

    private void GetShot()
    {
        if (currentState != EnemyState.Combat)
        {
            OnStateEnter(EnemyState.Combat);
        }
    }

    private void HeardNoise()
    {
        damaged = true;
        if (currentState != EnemyState.Alert)
        {
            OnStateEnter(EnemyState.Alert);
        }
     
    }

    private void LookAtTarget()
    {
        Quaternion rotTarget = Quaternion.LookRotation(
            target.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, rotTarget, gameManager.rotationSpeed *
            Time.deltaTime);
    }
}