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
        
        public float turnForce = 1f;

        public float turning;

        // private Queue<Vector3> velocityQueue = new Queue<Vector3>();

        public GameManager Manager;

        private Transform currentTrackSegment;

        private void Update()
        {
           
        }

        void FixedUpdate()
        {
            Turn();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Ground"))
            {
                currentTrackSegment = other.gameObject.transform;
            }
            
            if (other.gameObject.CompareTag("Left1"))
            {
                rb.AddTorque(transform.up * turnForce, ForceMode.Force);
            }
            else if (other.gameObject.CompareTag("Left2"))
            {
                rb.AddTorque(transform.up * turnForce / 4, ForceMode.Force);
            }
            else if (other.gameObject.CompareTag("Right1"))
            {
                rb.AddTorque(-transform.up * turnForce, ForceMode.Force);
            }
            else if (other.gameObject.CompareTag("Right2"))
            {
                rb.AddTorque(-transform.up * turnForce / 4, ForceMode.Force);
            }
            else if (other.gameObject.CompareTag("MiddleLeft"))
            {
                rb.AddTorque(-transform.up * turnForce * 2f, ForceMode.Force);
            }
            else if (other.gameObject.CompareTag("MiddleRight"))
            {
                rb.AddTorque(transform.up * turnForce * 2f, ForceMode.Force);
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

        private void Turn()
        {
            Vector3 currentVelocity = transform.forward;
            currentVelocity.y = 0f;
            Vector3 targetVelocity = currentTrackSegment.transform.forward;
            targetVelocity.y = 0f;
            turning = Vector3.Dot( Vector3.Cross(targetVelocity, currentVelocity), Vector3.up) < 0 ? Vector3.Angle(currentVelocity, targetVelocity) : -Vector3.Angle(currentVelocity, targetVelocity);
            rb.AddTorque(transform.up * (turning *  turnForce), ForceMode.Force);
            // Debug.Log(targetVelocity + " " + currentVelocity);
        }

        
        // public void ResetPosition(Transform bicycle)
        // {
        //     StartCoroutine(ResetPositionCoroutine(bicycle));
        // }
        //
        // IEnumerator ResetPositionCoroutine(Transform bicycle)
        // {
        //     // while((bicycle.position - currentTrackSegment.GetChild(0).position).magnitude > 0.5f)
        //     // {
        //     //     bicycle.position = Vector3.Lerp(bicycle.position, currentTrackSegment.GetChild(0).position, Time.deltaTime);
        //     //     bicycle.rotation = Quaternion.Lerp(bicycle.rotation,  currentTrackSegment.rotation, Time.deltaTime);
        //     //     yield return null;
        //     // }
        //     
        // }
        
        private void SlowDown()
        {
            rb.gameObject.GetComponent<WheelchairController1D>().enabled = false;
            while (rb.velocity.magnitude > .2f)
            {
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 0.1f * Time.deltaTime);
                rb.gameObject.transform.rotation = Quaternion.identity;
            }

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ |
                             RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }

        // private void Reset()
        // {
        //     rb.transform.position = currentTrackSegment.transform.position + currentTrackSegment.transform.lossyScale / 2;
        //     rb.transform.LookAt(rb.transform.position + currentTrackSegment.transform.forward);
        // }
        
        
        // private void AverageVelocity()
        // {
        //     if (velocityQueue.Count < 8)
        //     {
        //         velocityQueue.Enqueue(rb.velocity.normalized);
        //     }
        //     else
        //     {
        //         velocityQueue.Dequeue();
        //         velocityQueue.Enqueue(rb.velocity.normalized);
        //     }
        //     
        // }
        //
        // private Vector3 GetAverageVelocity()
        // {
        //     if (velocityQueue.Count == 0) return Vector3.zero;
        //     Vector3 sum = Vector3.zero;
        //     foreach (var velocity in velocityQueue)
        //     {
        //         sum += velocity;
        //     }
        //
        //     sum /= velocityQueue.Count;
        //     return sum.normalized;
        // }

    }
}