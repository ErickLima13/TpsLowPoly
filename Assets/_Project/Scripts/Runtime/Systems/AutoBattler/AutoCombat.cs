using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.UI;

public enum AutoState
{
    Idle, Combat, Die
}

public class AutoCombat : MonoBehaviour
{
    public AutoState currentState;

    private WaveController waveController;
    private RaycastWeapon raycastWeapon;

    public float viewRadius;
   // public float attackSpeed; // velocidade de ataque
    public LayerMask targetMask;
    public Transform targetTransform;

    public HpController targetHp;
    public HpController myHp;

    public Transform[] originWeapons;
    //public int[] damageAmount; // dano
    public Rig aimLayer;

    public Image barTime;
    public float time;


    public bool isAttack;

    private void Start()
    {
        waveController = FindObjectOfType<WaveController>();
        raycastWeapon = GetComponent<RaycastWeapon>();
        myHp = GetComponent<HpController>();

        FindTarget();

        waveController.NewEnemyEvent += FindTarget;
    }
    private void Update()
    {
        if (time < myHp.attributes.attackSpeed && isAttack)
        {
            time += Time.deltaTime;
            barTime.fillAmount = time / myHp.attributes.attackSpeed;
        }

        UpdateState();
    }

    #region State Machine

    private void UpdateState()
    {
        switch (currentState)
        {
            case AutoState.Idle:

                break;
            case AutoState.Combat:

                break;
            case AutoState.Die:

                break;
        }
    }

    private void OnStateEnter(AutoState state)
    {
        StopAllCoroutines();
        currentState = state;

        switch (currentState)
        {
            case AutoState.Idle:
                StartCoroutine("Idle");
                break;
            case AutoState.Combat:
                time = 0;
                StartCoroutine("Combat");
                break;
            case AutoState.Die:
                break;
        }
    }

    private IEnumerator Idle()
    {
        targetTransform = null;
        aimLayer.weight = 0;
        yield return new WaitForSeconds(0.5f);
        FindTarget();
    }

    #endregion



    private void OnDestroy()
    {
        waveController.NewEnemyEvent -= FindTarget;
    }

    private void FindTarget()
    {
        Collider[] targetInViewRadius = Physics.OverlapSphere(
     transform.position, viewRadius, targetMask);

        if (targetInViewRadius.Length > 0)
        {
            targetTransform = targetInViewRadius[0].transform;
            targetHp = targetTransform.GetComponent<HpController>();
            transform.LookAt(targetTransform);
            OnStateEnter(AutoState.Combat);
            //StartCoroutine("Combat");
        }
        else
        {
            OnStateEnter(AutoState.Idle);
        }
    }

    private IEnumerator Combat()
    {
        while (!targetHp.isDead && !myHp.isDead)
        {
            isAttack = true;
            yield return new WaitForSeconds(myHp.attributes.attackSpeed);
            aimLayer.weight = 1;
            yield return new WaitForEndOfFrame();
            raycastWeapon.raycastDestination = targetTransform;
            raycastWeapon.StartFire(originWeapons[0], myHp.attributes.damageAmount[0]);
            time = 0;
            isAttack = false;
        }

        if (targetHp.isDead)
        {
            OnStateEnter(AutoState.Idle);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }




}
