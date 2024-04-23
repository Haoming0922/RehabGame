using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Bicycle
{
    public class BicycleUIUpdater : MonoBehaviour
    {
        private Transform bike;
        private Quaternion startRotation;
        private float startHeight;
        public CircularProgressBar speedometerProgressBar;
        public TextMeshProUGUI speedText;

        private void Start()
        {
            bike = transform.parent;
            startRotation = transform.rotation;
            startHeight = transform.position.y;
            // speedometerProgressBar = transform.Find("Speedometer").gameObject.GetComponent<CircularProgressBar>();
            // speedText = transform.Find("Speed").gameObject.GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            transform.rotation = startRotation;
            transform.position = bike.position;
            // transform.position = new Vector3(bike.position.x, startHeight, bike.position.z);
        }

        public void UpdateSpeedUI(float speed, float speedometer)
        {
            speedometerProgressBar.UpdateFillAmount(speedometer);
            speedText.text = (int) speed + "km/h";
        }
    }
}