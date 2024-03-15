using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Game.Util;

namespace Game.Sensor
{
    public class DumbbellPair : MonoBehaviour
    {
        public TextMeshProUGUI guide;
        public PairingAvatar avatar;
        public float CFWeightAcc;
        
        private string currentSensorAddress = string.Empty;
        private SensorPosition currentPairingPosition = SensorPosition.LEFT;
        private float rotaionYMax = 0;
        private float rotaionYStart = 0;
        private float rotaionYEnd = 0;
        
        private bool isDown = false;
        
        private void Start()
        {
            StartDumbbellPair();
        }
        
        public void StartDumbbellPair()
        {
            StartCoroutine(DumbbellPairCalibrate());
        }
        

        private void OnDisable()
        {
            SyncsenseSensorManager.OnSensorDataReceivedEvent -= DumbbellPairEvent;
        }

        private void OnDestroy()
        {
            SyncsenseSensorManager.OnSensorDataReceivedEvent -= DumbbellPairEvent;
        }
        
        
        private void DumbbellPairEvent(SensorDataReceived data)
        {
            // select the first moving sensor and initialize
            if (currentSensorAddress == string.Empty && Calculation.IsRaise(data))
            {
                currentSensorAddress = data.deviceAddress;
                rotaionYMax = 0;
                
                float ax = Mathf.Clamp(data.accX, -9.8f, 9.8f);
                rotaionYStart = Mathf.Acos(ax / 9.8f) * Mathf.Rad2Deg;
                
                rotaionYEnd = rotaionYStart;
            }
            
            // update angle
            if (currentSensorAddress == data.deviceAddress)
            {
                rotaionYEnd = Calculation.ComplementaryFilterRotationY(data.gyroY, data.accX, rotaionYEnd, CFWeightAcc);
                // Debug.Log("[Haoming] rotaionYEnd: " + rotaionYEnd + ", theta: " + theta);
                avatar.UpdateArmRotation(currentPairingPosition, rotaionYEnd - rotaionYStart);
            }

            // update max angle
            if (Mathf.Abs(rotaionYEnd - rotaionYStart) > rotaionYMax)
            {
                rotaionYMax = Mathf.Abs(rotaionYEnd - rotaionYStart);
            }
            
        }
        

        public IEnumerator DumbbellPairCalibrate()
        {
            guide.text = "Please move the sensors to wake them";
            yield return new WaitForSeconds(3f);

            UserConfig userconfig = new UserConfig();
            SensorPairingData sensorPairingData = new SensorPairingData(Exercise.DUMBBELL);

            yield return StartCoroutine(DumbbellPairCalibrateOneSide(SensorPosition.LEFT, userconfig, sensorPairingData));
            yield return StartCoroutine(DumbbellPairCalibrateOneSide(SensorPosition.RIGHT, userconfig, sensorPairingData));

            DataSaver.SaveData("Dumbbell.userconfig", userconfig);
            DataSaver.SaveData("Dumbbell.sensorpair", sensorPairingData);
            
            gameObject.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.Invoke();
        }



        private IEnumerator DumbbellPairCalibrateOneSide(SensorPosition position, UserConfig userconfig, SensorPairingData sensorPairingData)
        {
            currentPairingPosition = position;
            
            guide.text = "Please put down the sensors";
            
            avatar.FindArm();
            yield return StartCoroutine(avatar.PutArmDown());
            
            yield return new WaitForSeconds(4f);
            
            // float t = 0f;
            // float waitTime = 3f;
            // while (t < waitTime)
            // {
            //     if (isDown)
            //     {
            //         t += Time.deltaTime;
            //     }
            //     else t = 0;
            //     yield return null;
            // }
            
            SyncsenseSensorManager.OnSensorDataReceivedEvent -= DumbbellPairEvent;
            SyncsenseSensorManager.OnSensorDataReceivedEvent += DumbbellPairEvent;
            
            guide.text = "Please only raise " + currentPairingPosition.ToString().ToLower() + " side";
            yield return new WaitForSeconds(20f);
            guide.text = "Max Angle: " + (int) rotaionYMax;
            yield return new WaitForSeconds(3f);
            
            SyncsenseSensorManager.OnSensorDataReceivedEvent -= DumbbellPairEvent;
            
            userconfig.SetArmRotationAngle(currentPairingPosition, rotaionYMax);
            sensorPairingData.SetSensorAddress(currentPairingPosition, currentSensorAddress);
            sensorPairingData.SetSensorDirection(currentPairingPosition, RotationDirection.NULL);

            Reset();
        }

        private void Reset()
        {
            currentSensorAddress = string.Empty;
            rotaionYMax = 0;
            rotaionYStart = 0;
            rotaionYEnd = 0;
        }

    }
}