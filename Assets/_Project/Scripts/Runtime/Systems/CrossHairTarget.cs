using UnityEngine;

public class CrossHairTarget : MonoBehaviour
{
    private Camera cam;
    private Ray ray;
    private RaycastHit hitInfo;

    public Transform empty;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        ray.origin = cam.transform.position;
        ray.direction = cam.transform.forward;
        if (Physics.Raycast(ray, out hitInfo))
        {
            transform.position = hitInfo.point;
        }
        else
        {
            transform.position = empty.position;
        }
    
    }
}
