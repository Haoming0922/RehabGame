using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Game.Util;

namespace Game.Sensor
{
    public class SensorInput
    {
        public float value;
        private float baseValue = 0;
        
        private string sensorAddress = "";
        private float currentRotationRaw = 0;
        private RotationDirection direction = RotationDirection.NULL;

        private Queue<float> dataWindow = new Queue<float>();
        private int lowPassWindowSize = 8;
        public float averageValue { get; private set; }
        public bool IsMove { get; private set; }

        private float angleStart = 5f;
        private float angleEnd = 5f;

        private float CFWeightAcc = 0.13f;
        private float gravity = 9.8f;
        
        public SensorInput(MiniGame game, SensorPosition position, SensorPairingData pairingData)
        {
            switch (position)
            {
                case SensorPosition.LEFT:
                    direction = pairingData.leftSensorDirection;
                    sensorAddress = pairingData.leftSensorAddress;
                    gravity = pairingData.leftSensorGravity;
                    Debug.Log("Haoming: " + pairingData.leftSensorAddress);
                    SetDefaultBaseValue(game);
                    
                    break;
                case SensorPosition.RIGHT:
                    direction = pairingData.rightSensorDirection;
                    sensorAddress = pairingData.rightSensorAddress;
                    gravity = pairingData.rightSensorGravity;
                    SetDefaultBaseValue(game); // TODO: Fetch base value from DB
                    break;
            }
            
        }


        void SetDefaultBaseValue(MiniGame game)
        {
            if (game == MiniGame.Dumbbell) baseValue = 10f;
            if (game == MiniGame.Wheelchair) baseValue = 20f;
            if (game == MiniGame.Cycle) baseValue = 20f;
        }

        public void WheelchairControlEvent(SensorDataReceived sensorData)
        {
            // Debug.Log("Haoming: sensorAddress" + sensorAddress + 
            //           "Haoming: sensorData.deviceAddress" + sensorData.deviceAddress);
            
            if (sensorAddress == sensorData.deviceAddress)
            {
                WheelchairLowPassFiltRotation(sensorData);
                WheelchairRotationToGameInput();
            }
        }
        
        public void DumbbellControlEvent1(SensorDataReceived sensorData)
        {
            if (sensorAddress == sensorData.deviceAddress)
            {
                DumbbellLowPassFiltRotation(sensorData);
                DumbbellRotationToGameInput1();
            }
        }
        
        public void DumbbellControlEvent2(SensorDataReceived sensorData)
        {
            if (sensorAddress == sensorData.deviceAddress)
            {
                DumbbellLowPassFiltRotation(sensorData);
                DumbbellRotationToGameInput2(sensorData);
            }
        }

        private void DumbbellLowPassFiltRotation(SensorDataReceived sensorData)
        {
            // float ax = sensorData.accX;
            // if (ax < -gravity) ax = -gravity;
            // else if (ax > gravity) ax = gravity;
            //
            // if (Calculation.IsDownX(sensorData) && !Calculation.IsMove(sensorData)){
            //     angleStart = Mathf.Acos(ax / gravity) * Mathf.Rad2Deg;
            //     angleEnd = angleStart;
            // }
            // else
            // {
            //     angleEnd = Calculation.ComplementaryFilterRotationY(sensorData.gyroY, sensorData.accX, angleEnd, CFWeightAcc, gravity);
            // }
            
            angleEnd = Calculation.ComplementaryFilterRotationY(sensorData.gyroY, sensorData.accX, angleEnd, CFWeightAcc, gravity);
        
            if (dataWindow.Count < lowPassWindowSize)
            {
                dataWindow.Enqueue(angleEnd - angleStart);
            }
            else
            {
                dataWindow.Dequeue();
                dataWindow.Enqueue(angleEnd - angleStart);
            }
            
            averageValue = Calculation.AverageQueue(dataWindow);
        }

        private void DumbbellRotationToGameInput1()
        {
            averageValue = Mathf.Clamp(averageValue, 0, baseValue);
            value = averageValue / baseValue;
        }
        
        private void DumbbellRotationToGameInput2(SensorDataReceived sensorData) //JumpJump
        {
            float rotate = sensorData.gyroY * Mathf.Sign(sensorData.accX);
            if (rotate >= -15f) // up
            {
                averageValue = Mathf.Clamp(averageValue, 0, baseValue);
                value = averageValue / baseValue;
            }
            else // down
            {
                value = -1;
            }
        }
        
        private void WheelchairLowPassFiltRotation(SensorDataReceived sensorData)
        {
            float data = 0;
            
            switch (direction)
            {
                case RotationDirection.XPOSITIVE:
                case RotationDirection.XNEGATIVE:
                    data = sensorData.gyroX;
                    break;
                case RotationDirection.YPOSITIVE:
                case RotationDirection.YNEGATIVE:
                    data = sensorData.gyroY;
                    break;
                case RotationDirection.ZPOSITIVE:
                case RotationDirection.ZNEGATIVE:
                    data = sensorData.gyroZ;
                    break;
                default: break;
            }
            
            // Debug.Log("Haoming: data" + data);
            
            if (dataWindow.Count < lowPassWindowSize)
            {
                dataWindow.Enqueue(data);
            }
            else
            {
                dataWindow.Dequeue();
                dataWindow.Enqueue(data);
            }

            averageValue = Calculation.AverageQueue(dataWindow);
            
            // Debug.Log("Haoming: averageValue" + averageValue);
            
            IsMove = Calculation.IsMove(sensorData);
        }


        private void WheelchairRotationToGameInput()
        {
            switch (direction)
            {
                case RotationDirection.XPOSITIVE:
                case RotationDirection.ZPOSITIVE:
                case RotationDirection.YPOSITIVE:
                    if (Mathf.Abs(averageValue) > baseValue)
                    {
                        value = Mathf.Clamp(averageValue / baseValue, 0f, 1f);
                        value = Mathf.Sign(averageValue) * value;
                    }
                    else
                    {
                        value = 0;
                    }
                    break;
                case RotationDirection.XNEGATIVE:
                case RotationDirection.YNEGATIVE:
                case RotationDirection.ZNEGATIVE:
                    if (Mathf.Abs(averageValue) > baseValue)
                    {
                        value = Mathf.Clamp(averageValue / baseValue, 0f, 1f);
                        value = -Mathf.Sign(averageValue) * value;
                    }
                    else
                    {
                        value = 0;
                    }
                    break;
                default: break;
            }
        }

        
        public void CycleControlEvent(SensorDataReceived sensorData)
        {
            // Debug.Log("Haoming: sensorAddress" + sensorAddress + 
            //           "Haoming: sensorData.deviceAddress" + sensorData.deviceAddress);
            
            if (sensorAddress == sensorData.deviceAddress)
            {
                Debug.Log("Haoming: " + averageValue);
                averageValue = Calculation.AccSum(sensorData);
                value = Calculation.ToCycleInput(averageValue, baseValue);
            }
        }
        

        /*
     
        #region archive

        public void SetCalibration(SensorDataReceived data)
        {
            if (Mathf.Abs(data.gyroX) > Mathf.Abs(data.gyroY))
            {
                if (Mathf.Abs(data.gyroX) > Mathf.Abs(data.gyroZ)) // X
                {
                    direction = data.gyroX > 0 ? RotationDirection.XPOSITIVE :
                        RotationDirection.XNEGATIVE;
                    maxRotation = data.gyroX;
                }
                else // Z
                {
                    direction = data.gyroZ > 0 ? RotationDirection.ZPOSITIVE :
                        RotationDirection.ZNEGATIVE;
                    maxRotation = data.gyroZ;
                }
            }
            else
            {
                if (Mathf.Abs(data.gyroY) > Mathf.Abs(data.gyroZ)) // Y
                {
                    direction = data.gyroY > 0 ? RotationDirection.YPOSITIVE :
                        RotationDirection.YNEGATIVE;
                    maxRotation = data.gyroY;
                }
                else // Z
                {
                    direction = data.gyroZ > 0 ? RotationDirection.ZPOSITIVE :
                        RotationDirection.ZNEGATIVE;
                    maxRotation = data.gyroZ;
                }
            }
        }

        #endregion
         */
        
        
    }

}