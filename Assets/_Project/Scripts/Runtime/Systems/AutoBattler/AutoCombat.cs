using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AutoCombat : MonoBehaviour
{
    private WaveController waveController;
    private RaycastWeapon raycastWeapon;

    public float viewRadius;
    public float attackSpeed;
    public LayerMask targetMask;
    public Transform targetTransform;

    public HpController targetHp;
    public HpController myHp;

    public Transform[] originWeapons;
    public int[] damageAmount;
    public Rig aimLayer;

    private void Start()
    {
        waveController = FindObjectOfType<WaveController>();
        raycastWeapon = GetComponent<RaycastWeapon>();
        myHp = GetComponent<HpController>();

        FindTarget();

        waveController.NewEnemyEvent += FindTarget;
    }

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
            StartCoroutine("Combat");
        }
    }

    private IEnumerator Combat()
    {
        while (!targetHp.isDead && !myHp.isDead)
        {
            yield return new WaitForSeconds(attackSpeed);
            aimLayer.weight = 1;
            yield return new WaitForEndOfFrame();
            raycastWeapon.raycastDestination = targetTransform;
            raycastWeapon.StartFire(originWeapons[0], damageAmount[0]);
        }

        if (targetHp.isDead)
        {
            targetTransform = null;
            aimLayer.weight = 0;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }




}
