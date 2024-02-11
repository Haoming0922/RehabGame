using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;

public class WheelchairController : MonoBehaviour
{
    public GameObject leftWheel;
    public GameObject rightWheel;
    public Rigidbody wheelchairRigidbody; // Reference to the wheelchair's Rigidbody
    public float maxLeftWheelRotationSpeed = 30f; // Maximum rotation speed for left wheel
    public float maxRightWheelRotationSpeed = 30f; // Maximum rotation speed for right wheel
    
    private bool isRunning = false;
    
    private float leftSpeed = 0;
    private float rightSpeed = 0;
    private float leftInput = 0;
    private float rightInput = 0;

    void Update()
    {
        StartCoroutine(GetSensorInput());
        
        // // Add keyboard controls
        // leftInput = GetKeyboardInput(KeyCode.Q, KeyCode.A); // Test on "Q" and "A" keys
        // rightInput = GetKeyboardInput(KeyCode.P, KeyCode.L); // Test on "P" and "L" keys

        leftSpeed = leftInput * maxLeftWheelRotationSpeed;
        rightSpeed = rightInput * maxRightWheelRotationSpeed;

        RotateWheel(leftWheel, leftSpeed);
        RotateWheel(rightWheel, rightSpeed);

        // Now you can use leftWheelSpeed and rightWheelSpeed to determine the movement and turning
        ApplyMovement(leftSpeed, rightSpeed);
    }

    IEnumerator GetSensorInput()
    {
        isRunning = true;
        while (true)
        {
            leftInput = GameDataManager.Instance.GetData("Left", Calculation.ToRotationData);
            rightInput = GameDataManager.Instance.GetData("Right", Calculation.ToRotationData);
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    
    // Function to get keyboard input
    private float GetKeyboardInput(KeyCode forwardKey, KeyCode backwardKey)
    {
        float input = 0;
        if (Input.GetKey(forwardKey))
        {
            input = 1;
        }
        else if (Input.GetKey(backwardKey))
        {
            input = -1;
        }
        return input;
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
            wheelchairRigidbody.AddRelativeForce(movement, ForceMode.Force);
            wheelchairRigidbody.AddTorque(turning, ForceMode.Force);
        }
    }
}