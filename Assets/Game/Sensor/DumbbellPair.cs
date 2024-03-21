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
        private Dictionary<string, float> restGravity = new Dictionary<string, float>();
        private Queue<SensorDataReceived> dataQueue = new Queue<SensorDataReceived>();
        private int windowSize = 8;
        
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
                
                dataQueue.Clear();

                float ax = data.accX;
                if (ax < -restGravity[currentSensorAddress]) ax = -restGravity[currentSensorAddress];
                else if (ax > restGravity[currentSensorAddress]) ax = restGravity[currentSensorAddress];
                
                rotaionYStart = Mathf.Acos(ax / restGravity[currentSensorAddress]) * Mathf.Rad2Deg;
                
                if (rotaionYStart > 90f)
                {
                    // X axis points upwards, set rotation back to “start from 0”
                    // only use for visualizing arm raising, for game input just use Math.Abs()
                    restGravity[currentSensorAddress] *= -1;
                    rotaionYStart = 180 - rotaionYStart;
                }
                
                rotaionYEnd = rotaionYStart;
            }
            
            // update angle
            if (currentSensorAddress == data.deviceAddress)
            {
                SensorDataReceived filterData = LowPassFilter(data);
                rotaionYEnd = Calculation.ComplementaryFilterRotationY(filterData.gyroY, filterData.accX, rotaionYEnd, CFWeightAcc, restGravity[currentSensorAddress]);
                Debug.Log("[Haoming] rotaionYEnd: " + rotaionYEnd + ", rotaionYStart: " + rotaionYStart);
                avatar.UpdateArmRotation(currentPairingPosition, rotaionYEnd - rotaionYStart);
            }

            // update max angle
            if (Mathf.Abs(rotaionYEnd - rotaionYStart) > rotaionYMax)
            {
                rotaionYMax = Mathf.Abs(rotaionYEnd - rotaionYStart);
            }
            
        }



        private SensorDataReceived LowPassFilter(SensorDataReceived data)
        {
            if (dataQueue.Count < windowSize)
            {
                dataQueue.Enqueue(data);
            }
            else
            {
                dataQueue.Dequeue();
                dataQueue.Enqueue(data);
            }

            return Calculation.AverageQueue(dataQueue);
        }
        

        public IEnumerator DumbbellPairCalibrate()
        {
            guide.text = "Please move the sensors to wake them";
            yield return new WaitForSeconds(3f);

            UserConfig userconfig = new UserConfig();
            SensorPairingData sensorPairingData = new SensorPairingData(Exercise.DUMBBELL);

            guide.text = "Please hold sensors still and DO NOT move";
            yield return new WaitForSeconds(3f);
            yield return StartCoroutine(DumbbellPairCalibrateRest());
            
            yield return StartCoroutine(DumbbellPairCalibrateOneSide(SensorPosition.LEFT, userconfig, sensorPairingData));
            yield return StartCoroutine(DumbbellPairCalibrateOneSide(SensorPosition.RIGHT, userconfig, sensorPairingData));

            DataSaver.SaveData("Dumbbell.userconfig", userconfig);
            DataSaver.SaveData("Dumbbell.sensorpair", sensorPairingData);
            
            gameObject.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.Invoke();
        }



        private void DumbbellPairRestEvent(SensorDataReceived data)
        {
            if (!restGravity.TryAdd(data.deviceAddress, Calculation.GetRestGravity(data)))
            {
                restGravity[data.deviceAddress] = ( restGravity[data.deviceAddress] + Calculation.GetRestGravity(data)) / 2f;
            }
        }
        

        private IEnumerator DumbbellPairCalibrateRest()
        {
            SyncsenseSensorManager.OnSensorDataReceivedEvent += DumbbellPairRestEvent;

            float t = 0f;
            while (t < 5f)
            {
                t += Time.deltaTime;
                yield return null;
            }

            SyncsenseSensorManager.OnSensorDataReceivedEvent -= DumbbellPairRestEvent;

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
            sensorPairingData.SetSensorGravity(currentPairingPosition, Mathf.Abs(restGravity[currentSensorAddress]));
            
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