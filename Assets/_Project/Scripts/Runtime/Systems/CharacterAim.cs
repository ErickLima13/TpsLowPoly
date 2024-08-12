using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterAim : MonoBehaviour
{
    public float turnSpeed = 15f;
    private Camera mainCamera;

    public RaycastWeapon raycastWeapon;

    [Header("Mira")]
    public float aimDuration = 0.2f;
    private Rig rigLayerWepPose;
    private bool isAim;

    [Header("Rig Builder Manager")]
    public RigBuilder rigBuilder;
    public Vector2[] idRigLayer; // eixo x arma, eixo y a layer

    [Header("Weapon Manager")]
    public int idWeapon;
    public GameObject[] weapons;
    public Rig[] RlWeaponAim;
    public Transform[] raycastOrigins;

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

        if (Input.GetKeyDown(KeyCode.R))
        {
            Unequip();
        }

        if (Input.mouseScrollDelta.y > 0)
        {
            InputChangeWeapon(true);
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            InputChangeWeapon(false);
        }

        if (rigLayerWepPose != null)
        {
            if (isAim)
            {
                rigLayerWepPose.weight +=
                    Time.deltaTime / aimDuration;

                if (Input.GetMouseButtonDown(0)
                    && rigLayerWepPose.weight.Equals(1))
                {
                    raycastWeapon.StartFire(raycastOrigins[idWeapon]);
                }
            }
            else
            {
                rigLayerWepPose.weight -=
                    Time.deltaTime / aimDuration;
            }
        }
    }

    private void InputChangeWeapon(bool value)
    {
        // pensar como incluir o metodo unequip nesse metodo
        
        if (value)
        {
            idWeapon++;
            if (idWeapon > weapons.Length - 1)
            {
                idWeapon = 0;
            }
        }
        else
        {
            idWeapon--;
            if (idWeapon < 0)
            {
                idWeapon = weapons.Length - 1;
            }
        }


        ChangeWeapon(idWeapon);
    }

    private void FixedUpdate()
    {
        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp
            (transform.rotation,
            Quaternion.Euler(0, yawCamera, 0),
            turnSpeed * Time.fixedDeltaTime);
    }

    private void ChangeWeapon(int idw)
    {
        for (int i = 0; i < idRigLayer.Length; i++)
        {
            Vector2 id = idRigLayer[i];
            int idArma = (int)id.x;
            int idLayer = (int)id.y;

            if (id.x != idw)
            {
                rigBuilder.layers[(int)id.y].active = false;
            }
            else if (id.x == idw)
            {
                rigBuilder.layers[(int)id.y].active = true;
            }
        }

        foreach (GameObject w in weapons)
        {
            w.SetActive(false);
        }

        weapons[idw].SetActive(true);
        rigLayerWepPose = RlWeaponAim[idw];
        rigLayerWepPose.weight = 0;
    }

    private void Unequip()
    {
        for (int i = 0; i < idRigLayer.Length; i++)
        {
            Vector2 id = idRigLayer[i];
            rigBuilder.layers[(int)id.y].active = false;
        }
        foreach (GameObject w in weapons)
        {
            w.SetActive(false);
        }
    }
}
