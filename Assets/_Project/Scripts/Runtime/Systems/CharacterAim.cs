using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterAim : MonoBehaviour
{
    public float turnSpeed = 15f;
    private Camera mainCamera;

    [Header("Mira")]
    public float aimDuration = 0.2f;
    public Rig rigLayerWepPos2;
    private bool isAim;

    private void Start()
    {
        mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isAim = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isAim = false;
        }

        if (isAim)
        {
            rigLayerWepPos2.weight += 
                Time.deltaTime / aimDuration;
        }
        else
        {
            rigLayerWepPos2.weight -=
                Time.deltaTime / aimDuration;
        }
    }

    private void FixedUpdate()
    {
        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp
            (transform.rotation,
            Quaternion.Euler(0, yawCamera, 0),
            turnSpeed * Time.fixedDeltaTime);

    }
}
