using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class WheelchairController : MonoBehaviour
{
    public GameObject leftWheel;
    public GameObject rightWheel;
    public Rigidbody wheelchairRigidbody; // Reference to the wheelchair's Rigidbody
    public float maxLeftWheelRotationSpeed = 25f; // Maximum rotation speed for left wheel
    public float maxRightWheelRotationSpeed = 25f; // Maximum rotation speed for right wheel
    public float forwardFactor;
    public float turningFactor;
    
    private float leftSpeed = 0;
    private float rightSpeed = 0;
    private float leftInput = 0;
    private float rightInput = 0;

    public TextMeshProUGUI textLeft;
    public TextMeshProUGUI textRight;

    private WheelChairInput input; 
    private InputAction leftAction;
    private InputAction rightAction;
    
    private void Awake()
    {
        // StartCoroutine(GetSensorInput());
        input = new WheelChairInput();
        leftAction = input.Wheelchair.LeftInput;
        rightAction = input.Wheelchair.RightInput;
        leftAction.Enable();
        rightAction.Enable();
    }

    void Update()
    {
        // leftInput = leftAction.ReadValue<Vector2>()[1];
        // rightInput = rightAction.ReadValue<Vector2>()[1];
        
        leftInput = leftAction.ReadValue<float>();
        rightInput = rightAction.ReadValue<float>();
        
        Debug.Log("Left: " + leftInput + ", Right: " + rightInput);
        textLeft.text = "Left: " + leftInput;
        textRight.text = "Right: " + rightInput;
     
        leftSpeed = leftInput * maxLeftWheelRotationSpeed;
        rightSpeed = rightInput * maxRightWheelRotationSpeed;

        RotateWheel(leftWheel, leftSpeed);
        RotateWheel(rightWheel, rightSpeed);

        // Now you can use leftWheelSpeed and rightWheelSpeed to determine the movement and turning
        ApplyMovement(leftSpeed, rightSpeed);
    }
    

    // IEnumerator GetSensorInput()
    // {
    //     while (true)
    //     {
    //         leftInput = GameDataManager.Instance.GetData("Left", Calculation.ToRotationData);
    //         rightInput = GameDataManager.Instance.GetData("Right", Calculation.ToRotationData);
    //         
    //         yield return new WaitForSeconds(0.05f);
    //     }
    // }

    // private void OnLeftInput(InputValue value)
    // {
    //     leftInput = value.Get<float>();
    //     leftInput = value.Get<Vector2>()[1];
    // }
    // private void OnRightInput(InputValue value)
    // {
    //     rightInput = value.Get<float>();
    //     rightInput = value.Get<Vector2>()[1];
    // }
    
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