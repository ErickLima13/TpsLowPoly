using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hitInfo;
    public Transform raycastDestination;

    public void StartFire(Transform raycastOrigin)
    {
        ray.origin = raycastOrigin.position;
        ray.direction = raycastDestination.position - raycastOrigin.position;

        if (Physics.Raycast(ray, out hitInfo))
        {
            Debug.DrawLine(ray.origin, hitInfo.point, Color.green, 1f);
        }
    }
}
