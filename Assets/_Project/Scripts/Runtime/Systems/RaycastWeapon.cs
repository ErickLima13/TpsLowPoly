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

    private void Start()
    {
        if (isEnemy)
        {
            enemy = GetComponent<EnemyAI>();
        }
    }

    public void StartFire(Transform raycastOrigin, int valueDamage)
    {
        if (raycastDestination == null) // minha mudança
        {
            return;
        }

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
            FeedbackHit(valueDamage);
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
}
