using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class WheelchairController2 : MonoBehaviour
{
    public GameObject leftWheel;
    public GameObject rightWheel;
    public Rigidbody wheelchairRigidbody; // Reference to the wheelchair's Rigidbody
    public float maxLeftWheelRotationSpeed = 25f; // Maximum rotation speed for left wheel
    public float maxRightWheelRotationSpeed = 25f; // Maximum rotation speed for right wheel
    public float forwardFactor;
    public float turningFactor;

    // final input
    private float leftInput = 0;
    private float rightInput = 0;

    public TextMeshProUGUI textLeft;
    public TextMeshProUGUI textRight;

    private WheelChairInput input; 
    private InputAction leftPositionAction;
    private InputAction rightPositionAction;
    private InputAction leftAccelerationAction;
    private InputAction rightAccelerationAction;
    private InputAction leftSetLocationAction;
    private InputAction rightSetLocationAction;

    public Vector3 leftOrigin = new Vector3(0, 0, 0);
    public Vector3 rightOrigin = new Vector3(0, 0, 0); 
    public Vector3 leftLastPosition = new Vector3(0, 0, 0);
    public Vector3 rightLastPosition = new Vector3(0, 0, 0); 
    
    private void Awake()
    {
        input = new WheelChairInput();
        leftPositionAction = input.HandControllerSensor.LeftPosition;
        rightPositionAction = input.HandControllerSensor.RightPosition;
        leftAccelerationAction = input.HandControllerSensor.LeftAcceleration;
        rightAccelerationAction = input.HandControllerSensor.RightAcceleration;
        leftSetLocationAction = input.HandControllerSensor.LeftSetLocation;
        rightSetLocationAction = input.HandControllerSensor.RightSetLocation;
    }

    private void OnEnable()
    {
        leftPositionAction.Enable();
        rightPositionAction.Enable();
        leftAccelerationAction.Enable();
        rightAccelerationAction.Enable();
        leftSetLocationAction.Enable();
        rightSetLocationAction.Enable();
    }

    private void OnDisable()
    {
        leftPositionAction.Disable();
        rightPositionAction.Disable();
        leftAccelerationAction.Disable();
        rightAccelerationAction.Disable();
        leftSetLocationAction.Disable();
        rightSetLocationAction.Disable();
    }

    void Update()
    {
        Vector3 leftPosition = leftPositionAction.ReadValue<Vector3>();
        leftPosition.x = 0;
        Vector3 rightPosition = rightPositionAction.ReadValue<Vector3>();
        rightPosition.x = 0;
        Vector3 leftAcceleration = leftAccelerationAction.ReadValue<Vector3>();
        leftAcceleration.x = 0;
        Vector3 rightAcceleration = rightAccelerationAction.ReadValue<Vector3>();
        rightAcceleration.x = 0;

        if (leftSetLocationAction.IsPressed()) leftOrigin = leftPosition;
        if (rightSetLocationAction.IsPressed()) rightOrigin = rightPosition;
        Debug.Log(
            "leftOrigin: " + leftOrigin + ", rightOrigin: " + rightOrigin
        );

        if (leftAcceleration.magnitude > 1.5)
            leftInput = Vector3.Cross(leftLastPosition - leftOrigin, leftPosition - leftOrigin).normalized.x;
        else leftInput = 0;
        if (rightAcceleration.magnitude > 1.5)
            rightInput = Vector3.Cross(rightLastPosition - rightOrigin, rightPosition - rightOrigin).normalized.x;
        else rightInput = 0;

        leftLastPosition = leftPosition;
        rightLastPosition = rightPosition;
        
        Debug.Log(
            "leftPosition: " + leftPosition + ", rightPosition: " + rightPosition +
                    ", leftAcceleration: " + leftAcceleration + ", rightAcceleration: " + rightAcceleration +
                    ", leftInput: " + leftInput + ", rightInput: " + rightInput
        );

        textLeft.text = "Left: " + leftInput;
        textRight.text = "Right: " + rightInput;
     
        float leftSpeed = leftInput * maxLeftWheelRotationSpeed;
        float rightSpeed = rightInput * maxRightWheelRotationSpeed;

        RotateWheel(leftWheel, leftSpeed);
        RotateWheel(rightWheel, rightSpeed);

        // Now you can use leftWheelSpeed and rightWheelSpeed to determine the movement and turning
        ApplyMovement(leftSpeed, rightSpeed);
    }
    
    
    
    // Function to rotate the wheel
    private void RotateWheel(GameObject wheel, float speed)
    {
        // Assuming the right vector points outwards, perpendicular from the wheel
        // Positive speed should rotate forward, negative speed should rotate backward
        wheel.transform.Rotate(wheel.transform.right, speed * Time.deltaTime);
    }

    // Function to apply movement and turning
    private void ApplyMovement(float leftWheelSpeed, float rightWheelSpeed)
    {
        // Determine the movement direction and turning based on the wheel speeds
        // If both wheels go forward/backward at the same speed, wheelchair should move straight
        // If one wheel moves and the other doesn't, wheelchair should turn

        // Forward movement vector
        Vector3 movement = Vector3.forward * ((leftWheelSpeed + rightWheelSpeed) / 2.0f);

        // Turning vector - this will determine how much the wheelchair should turn
        Vector3 turning = transform.up * (leftWheelSpeed - rightWheelSpeed) / 2.0f;

        // Apply the movement and turning to the wheelchair's Rigidbody
        if (wheelchairRigidbody != null && !wheelchairRigidbody.isKinematic)
        {
            wheelchairRigidbody.AddRelativeForce(movement * forwardFactor, ForceMode.Force);
            wheelchairRigidbody.AddTorque(turning * turningFactor, ForceMode.Force);
        }
    }
}