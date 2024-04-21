using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class markFollow : MonoBehaviour
{
    private Transform bike;
    private Quaternion startRotation;
    private float startHeight;
    private void Start()
    {
        bike = transform.parent;
        startRotation = transform.rotation;
        startHeight = transform.position.y;
    }

    private void Update()
    {
        transform.rotation = startRotation;
        transform.position = new Vector3(bike.position.x, startHeight, bike.position.z);
    }
}
