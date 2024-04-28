using System;
using System.Collections;
using System.Collections.Generic;
using Game.Sensor;
using UnityEngine;

namespace Game.Bicycle
{

    public class TurnController : MonoBehaviour
    {
        public Rigidbody rb;
        public WheelCollider wheel;
        public float horizontalInput = 0f;
        float turnSmoothing = 0.1f;
        private float currentSteeringAngle = 0f;
        
        private Queue<Vector3> velocityQueue = new Queue<Vector3>();

        public GameManager Manager;
        
        private void Start()
        {
            wheel = GetComponent<WheelCollider>();
        }

        private void Update()
        {
            AverageVelocity();
        }

        void FixedUpdate()
        {
            currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, horizontalInput, turnSmoothing);
            // currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, maxSteeringAngle * horizontalInput, turnSmoothing);
            wheel.steerAngle = currentSteeringAngle;
            Debug.Log(wheel.steerAngle);
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Ground"))
            {
                Turn(other.gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Finish"))
            {
                SlowDown();

                if (gameObject.CompareTag("Player"))
                {
                    if (!Manager.ghostWin) Manager.playerWin = true;
                    Manager.gameEndEvent?.Invoke();
                }
                else
                {
                    if(!Manager.playerWin) Manager.ghostWin = true;
                }
            }
        }

        private void SlowDown()
        {
            rb.gameObject.GetComponent<BicycleVehicle>().enabled = false;
            while (rb.velocity.magnitude > 1f)
            {
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 0.2f * Time.deltaTime);
                rb.gameObject.transform.rotation = Quaternion.identity;
            }

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ |
                                        RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }
        
        
        private void Turn(GameObject go)
        {
            Vector3 targetVelocity = go.transform.forward;
            targetVelocity.y = 0f;
            Vector3 currentVelocity = GetAverageVelocity();
            currentVelocity.y = 0f;
            horizontalInput = Vector3.Dot(Vector3.Cross(targetVelocity, currentVelocity), Vector3.up) < 0
                ? Vector3.Angle(currentVelocity, targetVelocity)
                : -Vector3.Angle(currentVelocity, targetVelocity);
            Debug.Log(targetVelocity + " " + currentVelocity);
        }

        private void AverageVelocity()
        {
            if (velocityQueue.Count < 10)
            {
                velocityQueue.Enqueue(rb.velocity.normalized);
            }
            else
            {
                velocityQueue.Dequeue();
                velocityQueue.Enqueue(rb.velocity.normalized);
            }
        }

        private Vector3 GetAverageVelocity()
        {
            if (velocityQueue.Count == 0) return Vector3.zero;
            Vector3 sum = Vector3.zero;
            foreach (var velocity in velocityQueue)
            {
                sum += velocity;
            }

            sum /= velocityQueue.Count;
            return sum.normalized;
        }

    }

}