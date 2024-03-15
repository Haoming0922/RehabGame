using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Game.Util;

namespace Game.Sensor
{
    public class RotationController
    {
        public float value = 0;
        private string sensorAddress = "";
        private float currentRotationRaw = 0;
        private float maxRotationAngle = 0;
        private RotationDirection direction = RotationDirection.NULL;

        private Queue<float> dataWindow = new Queue<float>();
        private int lowPassWindowSize = 8;
        public float averageValue { get; private set; }

        private float rotationThreshold = 20f;
        public bool IsMove { get; private set; }


        public RotationController(SensorPosition position, SensorPairingData pairingData, UserConfig userConfig)
        {
            switch (position)
            {
                case SensorPosition.LEFT:
                    direction = pairingData.leftSensorDirection;
                    sensorAddress = pairingData.leftSensorAddress;
                    maxRotationAngle = userConfig.maxLeftArmRotationAngle;
                    break;
                case SensorPosition.RIGHT:
                    direction = pairingData.rightSensorDirection;
                    sensorAddress = pairingData.rightSensorAddress;
                    maxRotationAngle = userConfig.maxRightArmRotationAngle;
                    break;
            }
            
        }

        public void WheelchairControlEvent(SensorDataReceived sensorData)
        {
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
                DumbbellRotationToGameInput2();
            }
        }

        private void DumbbellLowPassFiltRotation(SensorDataReceived sensorData)
        {
            float data = sensorData.gyroX * Mathf.Sign(sensorData.accX);
            
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
            
            IsMove = Calculation.IsMove(sensorData);
        }

        private void DumbbellRotationToGameInput1()
        {
            currentRotationRaw += averageValue;
            currentRotationRaw = Mathf.Clamp(currentRotationRaw, 0, maxRotationAngle);
            value = currentRotationRaw / maxRotationAngle;
        }
        
        private void DumbbellRotationToGameInput2() //JumpJump
        {
            if (averageValue >= -10f)
            {
                currentRotationRaw += averageValue;
                currentRotationRaw = Mathf.Clamp(currentRotationRaw, 0, maxRotationAngle);
                value = currentRotationRaw / maxRotationAngle;
            }
            else
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
            
            IsMove = Calculation.IsMove(sensorData);
        }


        private void WheelchairRotationToGameInput()
        {
            switch (direction)
            {
                case RotationDirection.XPOSITIVE:
                case RotationDirection.ZPOSITIVE:
                case RotationDirection.YPOSITIVE:
                    if (Mathf.Abs(averageValue) > rotationThreshold)
                    {
                        value = averageValue;
                    }
                    else
                    {
                        value = 0;
                    }
                    break;
                case RotationDirection.XNEGATIVE:
                case RotationDirection.YNEGATIVE:
                case RotationDirection.ZNEGATIVE:
                    if (Mathf.Abs(averageValue) > rotationThreshold)
                    {
                        value = -averageValue;
                    }
                    else
                    {
                        value = 0;
                    }
                    break;
                default: break;
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