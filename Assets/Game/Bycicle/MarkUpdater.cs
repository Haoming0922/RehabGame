﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Bicycle
{
    public class MarkUpdater : MonoBehaviour
    {
        public Transform bike;
        private Quaternion startRotation;
        private float startHeight;
        
        private void Start()
        {
            startHeight = transform.position.y;
        }

        private void Update()
        {
            // transform.position = new Vector3(bike.position.x, startHeight, bike.position.z);
            transform.rotation = Quaternion.identity;
        }
    }
}