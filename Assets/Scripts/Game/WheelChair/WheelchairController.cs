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

    // final input
    private float leftInput = 0;
    private float rightInput = 0;

    public TextMeshProUGUI textLeft;
    public TextMeshProUGUI textRight;

    private WheelChairInput input; 
    private InputAction leftAction;
    private InputAction rightAction;
    
    private void Awake()
    {
        input = new WheelChairInput();
        leftAction = input.Move.LeftInput;
        rightAction = input.Move.RightInput;
    }

    private void OnEnable()
    {
        leftAction.Enable();
        rightAction.Enable();
    }

    private void OnDisable()
    {
        leftAction.Disable();
        rightAction.Disable();
    }

    void Update()
    {
        float leftInput1 = leftAction.ReadValue<Vector2>()[1];
        float rightInput1 = rightAction.ReadValue<Vector2>()[1];
        float leftInput2 = GameDataManager.Instance != null ? GameDataManager.Instance.GetData("Left", Calculation.ToRotationData) : 0;
        float rightInput2 = GameDataManager.Instance != null ? GameDataManager.Instance.GetData("Right", Calculation.ToRotationData): 0;

        leftInput = Mathf.Abs(leftInput1) > Mathf.Abs(leftInput2) ? leftInput1 : leftInput2;
        rightInput = Mathf.Abs(rightInput1) > Mathf.Abs(rightInput2) ? rightInput1 : rightInput2;
        
        Debug.Log("Left: " + leftInput + ", Right: " + rightInput);
        textLeft.text = "Left: " + leftInput;
        textRight.text = "Right: " + rightInput;
     
        float leftSpeed = leftInput * maxLeftWheelRotationSpeed;
        float rightSpeed = rightInput * maxRightWheelRotationSpeed;

        RotateWheel(leftWheel, leftSpeed);
        RotateWheel(rightWheel, rightSpeed);

        // Now you can use leftWheelSpeed and rightWheelSpeed to determine the movement and turning
        ApplyMovement(leftSpeed, rightSpeed);
    }

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