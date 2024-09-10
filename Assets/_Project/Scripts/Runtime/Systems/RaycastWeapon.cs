using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hitInfo;
    public Transform raycastDestination;

    public ParticleSystem bubbleMuzzle;
    public ParticleSystem HitBubble;

    public Animator aimAnim;

    public bool isEnemy;
    private EnemyAI enemy;

    public float radius;
    public LayerMask targetMask;

    private void Start()
    {
        if (isEnemy)
        {
            enemy = GetComponent<EnemyAI>();
        }
    }

    public void StartFire(Transform raycastOrigin, int valueDamage)
    {
        ray.origin = raycastOrigin.position;
        ray.direction = raycastDestination.position - raycastOrigin.position;

        bubbleMuzzle.transform.position = raycastOrigin.position;
        bubbleMuzzle.Play();

        if (aimAnim != null) // minha mudança
        {
            aimAnim.SetTrigger("shoot");
        }

        if (isEnemy && enemy.Hit())
        {
            FeedbackHit(valueDamage);
        }

        if (!isEnemy)
        {
            FeedbackSound();
            FeedbackHit(valueDamage);
        }

    }

    private void FeedbackSound()
    {
        Collider[] targetInRadius = Physics.OverlapSphere(
           transform.position, radius, targetMask);

        for (int i = 0; i < targetInRadius.Length; i++)
        {
            targetInRadius[i].gameObject.SendMessage("HeardNoise",SendMessageOptions.DontRequireReceiver);
        }
    }

    public void FeedbackHit(int valueDamage)
    {
        if (Physics.Raycast(ray, out hitInfo))
        {
            Debug.DrawLine(ray.origin, hitInfo.point, Color.green, 1f);

            HitBubble.transform.position = hitInfo.point;
            HitBubble.transform.forward = hitInfo.normal;
            HitBubble.Play();

            hitInfo.collider.gameObject.SendMessage("GetDamage",
                valueDamage, SendMessageOptions.DontRequireReceiver);
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, radius);
    //}
}
