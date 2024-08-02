using System;
using TMPro;
using UnityEngine;

public enum Side
{
    Left, Right
}

public enum Axel
{
    Front, Rear
}

public enum DriverType
{
    Front, Rear, Full
}

public enum GearType
{
    Automatic, Manual
}

[Serializable]
public struct Wheel
{
    public GameObject model;
    public WheelCollider collider;

    public Axel axel;
    public Side side;
}

public class CompletCarController : MonoBehaviour
{
    public Wheel[] wheels;
    public TextMeshProUGUI speedTxt;

    public Transform centerOfmass;

    public float downForce;
    public float brakeTorque;
    public float steerRadius;

    public bool isBrake;

    [Header("Engine")]
    public float totalPower;
    public DriverType driverType;
    public GearType gearType;
    public AnimationCurve enginePower;
    public float[] gears;
    public int gearId;
    public float[] maxRpm;
    public float minRpm;

    private float kph;
    [SerializeField] private float torque;
    [SerializeField] private float engineRpm;
    [SerializeField] private float whellsRpm;
    private float smoothTime = 0.01f;

    private Rigidbody rbCar;
    private Vector2 input;



    private void Start()
    {
        rbCar = GetComponent<Rigidbody>();
        rbCar.centerOfMass = centerOfmass.localPosition;
    }

    private void Update()
    {
        GetInput();
        Shifter();
    }

    private void GetInput()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isBrake = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isBrake = false;
        }
    }

    private void FixedUpdate()
    {
        SetTorque();
    }

    private void SetTorque()
    {
        kph = rbCar.velocity.magnitude * 3.6f;
        speedTxt.text = kph.ToString("N0") + " kmh";

        rbCar.AddForce(Vector3.down * downForce * kph);

        if (driverType == DriverType.Full)
        {
            torque = totalPower / 4;
        }
        else
        {
            torque = totalPower / 2;
        }

        foreach (Wheel w in wheels)
        {
            switch (driverType)
            {
                case DriverType.Full:
                    w.collider.motorTorque = input.y * torque;
                    break;
                case DriverType.Front:
                    if (w.axel == Axel.Front)
                    {
                        w.collider.motorTorque = input.y * torque;
                    }
                    break;
                case DriverType.Rear:
                    if (w.axel == Axel.Rear)
                    {
                        w.collider.motorTorque = input.y * torque;
                    }
                    break;
            }
        }

        SetBrake();
        Turn();
        AnimateWheels();
        CalculateEnginePower();


    }

    private void Shifter()
    {
        switch (gearType)
        {
            case GearType.Manual:
                if (Input.GetKeyDown(KeyCode.E) && gearId < gears.Length - 1)
                {
                    gearId++;
                }
                if (Input.GetKeyDown(KeyCode.Q) && gearId > 0)
                {
                    gearId--;
                }

                break;
            case GearType.Automatic:

                if (engineRpm > maxRpm[gearId] && gearId < gears.Length - 1)
                {
                    gearId++;
                }
                else if (engineRpm < minRpm && gearId > 0)
                {
                    gearId--;
                }

                break;

        }
    }

    private void WhellRpm()
    {
        float sum = 0;
        int r = 0;

        foreach (Wheel w in wheels)
        {
            sum += w.collider.rpm;
            r++;
        }

        whellsRpm = r != 0 ? sum / r : 0;


    }

    private void CalculateEnginePower()
    {
        WhellRpm();
        totalPower = enginePower.Evaluate
            (engineRpm) / gears[gearId] * input.magnitude;

        float velocity = 0;
        engineRpm = Mathf.SmoothDamp(
            engineRpm, 1000 + Mathf.Abs(whellsRpm)
            * 3.6f * gears[gearId], ref velocity, smoothTime);
    }

    private void SetBrake()
    {
        foreach (Wheel w in wheels)
        {
            if (isBrake)
            {
                w.collider.motorTorque = 0;
                w.collider.brakeTorque = brakeTorque;
            }
            else
            {
                w.collider.brakeTorque = 0;
            }
        }
    }

    private void Turn()
    {
        float r = steerRadius;
        if (kph >= 100)
        {
            r = 10;
        }
        else if (kph >= 80)
        {
            r = 8;
        }
        else if (kph >= 60)
        {
            r = 6;
        }
        else if (kph >= 40)
        {
            r = 4;
        }
        else
        {
            r = 2.5f;
        }

        foreach (Wheel w in wheels)
        {
            if (w.axel == Axel.Front)
            {
                if (input.x > 0) // direita
                {
                    switch (w.side)
                    {
                        case Side.Left:
                            w.collider.steerAngle =
                                Mathf.Rad2Deg * Mathf.Atan(
                                    2.55f /
                                    (r + (1.5f / 2)))
                                * input.x;
                            break;
                        case Side.Right:
                            w.collider.steerAngle =
                                Mathf.Rad2Deg * Mathf.Atan(
                                2.55f /
                                (r - (1.5f / 2)))
                                * input.x;
                            break;
                    }
                }
                else if (input.x < 0) // esquerda
                {
                    switch (w.side)
                    {
                        case Side.Left:
                            w.collider.steerAngle =
                                Mathf.Rad2Deg * Mathf.Atan(
                                    2.55f /
                                    (r - (1.5f / 2)))
                                * input.x;
                            break;
                        case Side.Right:
                            w.collider.steerAngle =
                                Mathf.Rad2Deg * Mathf.Atan(
                                2.55f /
                                (r + (1.5f / 2)))
                                * input.x;
                            break;
                    }
                }
                else
                {
                    w.collider.steerAngle = 0;
                }
            }
        }
    }

    private void AnimateWheels()
    {
        foreach (Wheel w in wheels)
        {
            Quaternion rot;
            Vector3 pos;
            w.collider.GetWorldPose(out pos, out rot);
            w.model.transform.position = pos;
            w.model.transform.rotation = rot;
        }
    }
}
