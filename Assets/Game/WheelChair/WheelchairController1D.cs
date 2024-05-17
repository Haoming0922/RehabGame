using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using Game.Wheelchair;
using Game.Util;
using Game.Sensor;
using Game.Bicycle;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

namespace Game.Wheelchair
{
    public class WheelchairController1D : MonoBehaviour
    {
        public bool isUser;
        public WheelchairTurnController TurnController;
        public WheelchairUIUpdater UIUpdater;
        private float maxSpeed = 10f;

        public SensorManager sensorManager;
        public GameManager gameManager;
        public ReHybAvatarController AvatarController;

        public GameObject leftWheel;
        public GameObject rightWheel;
        public Rigidbody wheelchairRigidbody; // Reference to the wheelchair's Rigidbody
        public float forwardFactor;
        public float turningFactor;

        private float maxWheelRotationSpeed = 15f; // Maximum rotation speed for wheel

        // final input
        private float leftInput = 0;
        private float rightInput = 0;

        private float avgInput;

        private float baseValue = 1;

        private WheelChairInput input;
        private InputAction leftAction;
        private InputAction rightAction;
        private InputAction joyStickAction;

        private float leftInput2;
        private float rightInput2;

        private void Awake()
        {
            input = new WheelChairInput();
            leftAction = input.Move.LeftInput;
            rightAction = input.Move.RightInput;
            joyStickAction = input.Move.JoyStick;
        }

        private void OnEnable()
        {
            leftAction.Enable();
            rightAction.Enable();
            joyStickAction.Enable();

            wheelchairRigidbody.mass = 30;
            wheelchairRigidbody.centerOfMass -= 3 * transform.up;
            if(isUser) wheelchairRigidbody.centerOfMass += transform.forward / 2.7f;
            else
            {
                wheelchairRigidbody.centerOfMass += transform.forward / 3;
            }
        }

        private void OnDisable()
        {
            leftAction.Disable();
            rightAction.Disable();
            joyStickAction.Disable();
        }

        void FixedUpdate()
        {
            if (gameManager.state == GameState.END)
            {
                wheelchairRigidbody.velocity = Vector3.Lerp(wheelchairRigidbody.velocity, Vector3.zero, Time.deltaTime);
                wheelchairRigidbody.isKinematic = true;
            }
            else if (gameManager.state == GameState.PLAY)
            {
                if (isUser)
                {
                    GetInput();
                    gameManager.UpdatePlayerData(wheelchairRigidbody.velocity.magnitude, leftInput2, rightInput2);
                }
                else AutoDrive();
            }

            Stable();
        }

        private void Update()
        {
            if (gameManager.state == GameState.PLAY) UpdateUI();
        }

        private void Stable()
        {
            wheelchairRigidbody.AddForce(-transform.up * (wheelchairRigidbody.velocity.magnitude), ForceMode.Force);
            
        }
        
        void AutoDrive()
        {
            float vereticallInput = 0;
            if (transform.position.z - gameManager.player.position.z > 20)
            {
                vereticallInput = Mathf.Min(.2f, avgInput);	
            }
            else if (transform.position.z - gameManager.player.position.z < -20)
            {
                vereticallInput = Mathf.Max(2f,avgInput);
            }
            else
            {
                vereticallInput = UnityEngine.Random.Range(1f, 2f);
            }
            
            wheelchairRigidbody.AddRelativeForce(vereticallInput * forwardFactor * Vector3.forward, ForceMode.Force);
            // wheelchairRigidbody.AddForce(baseValue * forwardFactor * TurnController.velocity, ForceMode.Force);
            // Vector3 currentVelocity = wheelchairRigidbody.velocity;
            // Vector3 targetVelocity  = currentVelocity.magnitude * new Vector3(TurnController.velocity.x, currentVelocity.y, TurnController.velocity.z).normalized;
            // wheelchairRigidbody.velocity = Vector3.Lerp(currentVelocity, targetVelocity, 5 * Time.deltaTime);
        }

        void GetInput()
        {
            // avgInput = 2;
            // wheelchairRigidbody.AddRelativeForce(avgInput * forwardFactor * Vector3.forward, ForceMode.Force);
            
            RotateWheel(leftWheel, avgInput * maxWheelRotationSpeed);
            RotateWheel(rightWheel, avgInput * maxWheelRotationSpeed);
            
            // return;
            
            // float leftInput1 = leftAction.ReadValue<Vector2>()[1];
            // float rightInput1 = rightAction.ReadValue<Vector2>()[1];
            leftInput2 = sensorManager.GetData(SensorPosition.LEFT);
            rightInput2 = sensorManager.GetData(SensorPosition.RIGHT);
            // Debug.Log("leftInput2: " + leftInput2 + ", rightInput2: " + rightInput2);

            // Vector2 joyStick = joyStickAction.ReadValue<Vector2>();
            // float leftInput3 = joyStick.y >= 0 ? joyStick.y + joyStick.x : joyStick.y - joyStick.x;
            // leftInput3 = Mathf.Sign(leftInput3) * Mathf.Min(Mathf.Abs(leftInput3), 1);
            // float rightInput3 = joyStick.y >= 0 ? joyStick.y - joyStick.x : joyStick.y + joyStick.x;
            // rightInput3 = Mathf.Sign(rightInput3) * Mathf.Min(Mathf.Abs(rightInput3), 1);

            // leftInput = Mathf.Abs(leftInput1) > Mathf.Abs(leftInput2) ? leftInput1 : leftInput2;
            // leftInput = Mathf.Abs(leftInput) > Mathf.Abs(leftInput3) ? leftInput : leftInput3;
            // rightInput = Mathf.Abs(rightInput1) > Mathf.Abs(rightInput2) ? rightInput1 : rightInput2;
            // rightInput = Mathf.Abs(rightInput) > Mathf.Abs(rightInput3) ? rightInput : rightInput3;

            avgInput = (leftInput2 + rightInput2) / 2;
            wheelchairRigidbody.AddRelativeForce(avgInput * forwardFactor * Vector3.forward, ForceMode.Force);
            
            // ApplyMovement(leftInput, rightInput);
        }


        private void UpdateUI()
        {
            float speed = Mathf.Clamp(wheelchairRigidbody.velocity.magnitude, 0, maxSpeed);
            UIUpdater.UpdateSpeedUI(speed, speed / maxSpeed);
            UIUpdater.UpdateForceUI(avgInput);
        }

        Vector3 RotateVector(Vector3 vector, float angleDegrees)
        {
            // Create a rotation Quaternion around the Y axis
            Quaternion rotation = Quaternion.Euler(0, angleDegrees, 0);

            // Apply the rotation to the vector
            return rotation * vector;
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
}