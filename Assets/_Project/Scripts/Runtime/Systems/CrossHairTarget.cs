using UnityEngine;

public class CrossHairTarget : MonoBehaviour
{
    private Camera cam;
    private Ray ray;
    private RaycastHit hitInfo;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        ray.origin = cam.transform.position;
        ray.direction = cam.transform.forward;
        Physics.Raycast(ray, out hitInfo);
        transform.position = hitInfo.point;
    }
}
