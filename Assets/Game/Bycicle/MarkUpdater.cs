using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Bicycle
{
    public class MarkUpdater : MonoBehaviour
    {
        private Transform bike;
        private Quaternion startRotation;
        private float startHeight;
        
        private void Start()
        {
            startHeight = transform.position.y;
        }

        private void Update()
        {
            transform.position = new Vector3(transform.position.x, startHeight, transform.position.z);
            transform.rotation = Quaternion.identity;
        }
    }
}