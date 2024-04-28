using System;
using System.Collections;
using System.Collections.Generic;
using Game.Bicycle;
using Game.Sensor;
using Unity.VisualScripting;
using UnityEngine;


namespace Game.Wheelchair
{
    public class WheelchairTurnController : MonoBehaviour
    {
        public Rigidbody rb;
        private float horizontalInput = 0f;
        float turnSmoothing = 0.05f;
        public float currentSteeringAngle = 0f;

        public Vector3 velocity;

        private Queue<Vector3> velocityQueue = new Queue<Vector3>();

        public GameManager Manager;
        private void Start()
        {
        }

        private void Update()
        {
            AverageVelocity();
        }

        void FixedUpdate()
        {
            // currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, horizontalInput, turnSmoothing);
            // // currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, maxSteeringAngle * horizontalInput, turnSmoothing);
            // Debug.Log(currentSteeringAngle);
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
                if (gameObject.CompareTag("Player"))
                {
                    Manager.playerWin = true;
                    Manager.gameEndEvent?.Invoke();
                }
                else
                {
                    Manager.playerWin = false;
                    gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    gameObject.GetComponent<WheelchairController1D>().enabled = false;
                }
            }
        }

        private void Turn(GameObject go)
        {
            Vector3 targetVelocity = go.transform.forward;
            targetVelocity.y = 0f;
            Vector3 currentVelocity = GetAverageVelocity();
            currentVelocity.y = 0f;
            velocity = targetVelocity;

            // velocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime).normalized;
            // horizontalInput = Vector3.Dot( Vector3.Cross(targetVelocity, currentVelocity), Vector3.up) < 0 ? Vector3.Angle(currentVelocity, targetVelocity) : -Vector3.Angle(currentVelocity, targetVelocity);
            // Debug.Log(targetVelocity + " " + currentVelocity);
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