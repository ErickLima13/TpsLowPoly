using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hitInfo;
    public Transform raycastDestination;

    public ParticleSystem bubbleMuzzle;
    public ParticleSystem HitBubble;

    public Animator aimAnim;

    public void StartFire(Transform raycastOrigin)
    {
        ray.origin = raycastOrigin.position;
        ray.direction = raycastDestination.position - raycastOrigin.position;

        bubbleMuzzle.transform.position = raycastOrigin.position;
        bubbleMuzzle.Play();
        aimAnim.SetTrigger("shoot");

        if (Physics.Raycast(ray, out hitInfo))
        {
            //Debug.DrawLine(ray.origin, hitInfo.point, Color.green, 1f);
            HitBubble.transform.position = hitInfo.point;
            HitBubble.transform.forward = hitInfo.normal;
            HitBubble.Play();
        }
    }
}
