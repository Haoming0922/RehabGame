using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Transform cube;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Configure LineRenderer
        lineRenderer.positionCount = 2; // Only two points (start and end)
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        // Set positions
        Vector3 startPoint = cube.GetChild(0).position;
        Vector3 endPoint = startPoint + Vector3.up * 10; // 10 units upwards

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }
}
