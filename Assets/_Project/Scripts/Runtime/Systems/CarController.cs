using UnityEngine;

public class CarController : MonoBehaviour
{
    public WheelCollider Fl;
    public WheelCollider Fr;
    public WheelCollider Rl;
    public WheelCollider Rr;

    public float torque;
    public float maxSteerAngle = 30;
    public float brakeTorque;

    private Vector2 input;

    [SerializeField] private bool isBrake;

    private void Update()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fl.motorTorque = 0;
            Fr.motorTorque = 0;
            Rl.motorTorque = 0;
            Rr.motorTorque = 0;

            Fl.brakeTorque = brakeTorque;
            Fr.brakeTorque = brakeTorque;
            Rl.brakeTorque = brakeTorque;
            Rr.brakeTorque = brakeTorque;

            isBrake = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isBrake = false;
            Fl.brakeTorque = 0;
            Fr.brakeTorque = 0;
            Rl.brakeTorque = 0;
            Rr.brakeTorque = 0;
        }
    }

    private void FixedUpdate()
    {
        if (!isBrake)
        {
            Fl.motorTorque = input.y * torque;
            Fr.motorTorque = input.y * torque;
            Rl.motorTorque = input.y * torque;
            Rr.motorTorque = input.y * torque;
        }
        float steerAngle = input.x * maxSteerAngle;
        Fl.steerAngle = steerAngle;
        Fr.steerAngle = steerAngle;
    }
}
