using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCamera : MonoBehaviour
{
    // ligar desligar farol alterar a variavel emission no material afrcemission
    // verificar se ele vai girar as rodas

    private Transform rootNode;
    private Transform cam;
    private Transform car;
    private Rigidbody rbCar;

    public float rotationThreshold = 1f;
    public float cameraStickenees = 10f;
    public float cameraRotationSpeed = 5f;


    private void Awake()
    {
        rootNode = this.transform;
        cam = GetComponentInChildren<Camera>().transform;
        rbCar = rootNode.parent.GetComponent<Rigidbody>();
        car = rootNode.parent.transform;
    }

    private void Start()
    {
        rootNode.parent = null;
    }

    private void FixedUpdate()
    {
        Quaternion lookAt;

        rootNode.position = Vector3.Lerp(rootNode.position,
            car.position, cameraStickenees * Time.fixedDeltaTime);

        if (rbCar.velocity.magnitude < rotationThreshold)
        {
            lookAt = Quaternion.LookRotation(car.forward);
        }
        else
        {
            lookAt = Quaternion.LookRotation(rbCar.velocity.normalized);
        }

        lookAt = Quaternion.Slerp(rootNode.rotation, lookAt,
            cameraRotationSpeed * Time.fixedDeltaTime);
        rootNode.rotation = lookAt; 
    }
}
