using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public enum ViewState
    {
        Primary, Secondary
    }

    public GameObject AIManager;

    [Range(0, 100)] public float viewRadius;
    [Range(0, 360)] public float viewAngle;
    [Range(0, 360)] public float viewAngleSecondary;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public List<Transform> visibleTargets = new List<Transform>();
    public List<Transform> visibleSecondary = new List<Transform>();

    public float viewTime;

    private void Start()
    {
        StartCoroutine(FindTarget());
    }

    private IEnumerator FindTarget()
    {
        while (true)
        {
            FindVisibleTarget();
            yield return new WaitForSeconds(viewTime);
        }
    }

    public Vector3 DirFromAngle(float angleInDegress)
    {
        angleInDegress += transform.eulerAngles.y;
        return new
            Vector3(Mathf.Sin(angleInDegress * Mathf.Deg2Rad),
          0, Mathf.Cos(angleInDegress * Mathf.Deg2Rad));
    }

    private void FindVisibleTarget()
    {
        visibleTargets.Clear();
        visibleSecondary.Clear();

        Collider[] targetInViewRadius = Physics.OverlapSphere(
            transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetInViewRadius.Length; i++)
        {
            Transform target = targetInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            Debug.DrawRay(transform.position, dirToTarget * viewAngle, Color.green);

            if (Vector3.Angle(transform.forward, dirToTarget) <
                viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position,
                    dirToTarget, distToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                    AIManager.SendMessage("IsVisible",
                        ViewState.Primary,
                        SendMessageOptions.DontRequireReceiver);
                }
            }
            else if (Vector3.Angle(transform.forward, dirToTarget) <
                viewAngleSecondary / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position,
                    dirToTarget, distToTarget, obstacleMask))
                {
                    visibleSecondary.Add(target);
                    AIManager.SendMessage("IsVisible",
                   ViewState.Secondary,
                   SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}
