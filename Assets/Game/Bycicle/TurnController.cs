using System;
using System.Collections;
using System.Collections.Generic;
using Game.Sensor;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    public Rigidbody rb;
    public WheelCollider wheel;
    public float horizontalInput = 0f;
    float turnSmoothing = 0.05f;
    private float currentSteeringAngle = 0f;

    public TrackGenerator trackGenerator;

    private Queue<Vector3> velocityQueue = new Queue<Vector3>();
    
    
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
        // Debug.Log(wheel.steerAngle);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Turn(other.gameObject);
        }
    }
    
    private void Turn(GameObject go)
    {
        Vector3 targetVelocity = go.transform.forward;
        targetVelocity.y = 0f;
        Vector3 currentVelocity = GetAverageVelocity();
        currentVelocity.y = 0f;
        horizontalInput = Vector3.Dot( Vector3.Cross(targetVelocity, currentVelocity), Vector3.up) < 0 ? Vector3.Angle(currentVelocity, targetVelocity) : -Vector3.Angle(currentVelocity, targetVelocity);
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
