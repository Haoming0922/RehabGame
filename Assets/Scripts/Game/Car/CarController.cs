using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    public Text textLeft;
    public Text textRight;
    
    float horizontalInput;
    float verticalInput;

    [SerializeField] float motorForce;
    [SerializeField] float maxRotation;
    [SerializeField] WheelCollider frontLeftWheelCollider;
    [SerializeField] WheelCollider frontRightWheelCollider;
    [SerializeField] WheelCollider rearLeftWheelCollider;
    [SerializeField] WheelCollider rearRightWheelCollider;

    [SerializeField] Transform frontLeftWheelTransform;
    [SerializeField] Transform frontRightWheelTransform;
    [SerializeField] Transform rearLeftWheelTransform;
    [SerializeField] Transform rearRightWheelTransform;
    
    [SerializeField] float turningThreshold;

    private bool isRunning = false;


    private void Update()
    {
        UpdateWheels();
        if(!isRunning) StartCoroutine(GetInput());
    }

    void FixedUpdate()
    {
        HandleMotor();
        HandleSteering();
    }
    
    

    IEnumerator GetInput()
    {
        isRunning = true;
        while (true)
        {
            
            // float left = GameDataManager.Instance.GetData("Left", Calculation.ToRacingData);
            // float right = GameDataManager.Instance.GetData("Right", Calculation.ToRacingData);
            // textLeft.text = "Left: " + left;
            // textRight.text = "Right: " + right;
            //
            // if (left - right > turningThreshold) horizontalInput = 1;
            // else if (left - right < -turningThreshold) horizontalInput = -1;
            // else horizontalInput = 0;
            //
            // if (left != 0 && right != 0) verticalInput = (left + right) / 2;
            // else verticalInput = 0;
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            yield return new WaitForSeconds(0.4f);
        }
    }

    void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;
    }

    void HandleSteering()
    {
        float currentRotation = maxRotation * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentRotation;
        frontRightWheelCollider.steerAngle = currentRotation;
    }

    void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }
    
}
