using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterAim : MonoBehaviour
{
    

    public float turnSpeed = 15f;
    private Camera mainCamera;
    private bool hasWeapon;

    [Header("Cinemachine Control")]
    public Cinemachine.CinemachineVirtualCamera playerCamera;
    public Cinemachine.CinemachineImpulseSource cameraShake;
    public Cinemachine.AxisState xAxis;
    public Cinemachine.AxisState yAxis;
    public Transform cameraLookAt;

    public RaycastWeapon raycastWeapon;

    [Header("Mira")]
    public float aimDuration = 0.2f;
    private Rig rigLayerWepPose;
    private bool isAim;

    [Header("Rig Builder Manager")]
    public RigBuilder rigBuilder;
    public Vector2[] idRigLayer; // eixo x arma, eixo y a layer

    // tudo da arma ir pra um scriptable object
    [Header("Weapon Manager")]
    public int idWeapon;
    public GameObject[] weapons;
    public int[] amountDamage;
    public float[] delayShoot;
    public bool[] isRapidFire;
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

        if (rigLayerWepPose != null && hasWeapon)
        {
            if (isAim)
            {
                rigLayerWepPose.weight +=
                    Time.deltaTime / aimDuration;

                if (Input.GetMouseButtonDown(0)
                    && rigLayerWepPose.weight.Equals(1))
                {
                    if (isRapidFire[idWeapon])
                    {
                        StartCoroutine("RapidFire");
                    }
                    else
                    {
                        FireWeapon();
                    }
                }

            }
            else
            {
                rigLayerWepPose.weight -=
                    Time.deltaTime / aimDuration;

                StopCoroutine("RapidFire");
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopCoroutine("RapidFire");
        }
    }

    private void FixedUpdate()
    {
       xAxis.Update(Time.fixedDeltaTime);
       yAxis.Update(Time.fixedDeltaTime);

        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp
            (transform.rotation,
            Quaternion.Euler(0, yawCamera, 0),
            turnSpeed * Time.fixedDeltaTime);
        
        cameraLookAt.eulerAngles =
            new Vector3(yAxis.Value, xAxis.Value, 0);
    }

    private IEnumerator RapidFire()
    {
        while (true)
        {
            FireWeapon();
            yield return new WaitForSeconds(delayShoot[idWeapon]);
        }
    }

    private void FireWeapon()
    {
        raycastWeapon.StartFire(raycastOrigins[idWeapon], amountDamage[idWeapon]);
        cameraShake.GenerateImpulse(playerCamera.transform.forward);
        yAxis.Value -= 0.5f;
    }

    private void InputChangeWeapon(bool value)
    {
        if (value)
        {
            idWeapon++;
        }
        else
        {
            idWeapon--;
        }

        ChangeWeapon(idWeapon);
    }

    private void ChangeWeapon(int idw)
    {
        if (idWeapon > weapons.Length - 1 || idWeapon < 0)
        {
            Unequip();
        }
        else
        {
            hasWeapon = true;

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
    }

    private void Unequip()
    {
        hasWeapon = false;

        for (int i = 0; i < idRigLayer.Length; i++)
        {
            Vector2 id = idRigLayer[i];
            rigBuilder.layers[(int)id.y].active = false;
        }
        foreach (GameObject w in weapons)
        {
            w.SetActive(false);
        }

        if (idWeapon < 0)
        {
            idWeapon = weapons.Length;
        }
        else if (idWeapon > weapons.Length - 1)
        {
            idWeapon = -1;
        }
    }
}

