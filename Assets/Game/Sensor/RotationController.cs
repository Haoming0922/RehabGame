using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Sensor
{
    public class RotationController
    {
        public float value = 0;

        private string sensorAddress = "";

        private int lowPassWindowSize = 8;
        public SensorDataReceived averageValue { get; private set; }
        private float rotationThreshold = 20f;

        private Queue<SensorDataReceived> dataWindow = new Queue<SensorDataReceived>();

        private float maxRotation = 1f;
        
        public RotationDirection direction { get; set; }
        public bool IsMove { get; private set; }


        public RotationController(SensorPosition position, SensorPairingData pairingData)
        {
            switch (position)
            {
                case SensorPosition.LEFT:
                    direction = pairingData.leftSensorDirection;
                    sensorAddress = pairingData.leftSensorAddress;
                    break;
                case SensorPosition.RIGHT:
                    direction = pairingData.rightSensorDirection;
                    sensorAddress = pairingData.rightSensorAddress;
                    break;
            }

            SyncsenseSensorManager.OnSensorDataReceivedEvent += RotationControlEvent;
        }
        
        private void OnDestroy()
        {
            SyncsenseSensorManager.OnSensorDataReceivedEvent -= RotationControlEvent;
        }
        

        public void RotationControlEvent(SensorDataReceived sensorData)
        {
            if (sensorAddress == sensorData.deviceAddress)
            {
                LowPassFiltRotation(sensorData);
                RotationToGameInput();
            }
        }

        private void LowPassFiltRotation(SensorDataReceived sensorData)
        {
            if (dataWindow.Count < lowPassWindowSize)
            {
                dataWindow.Enqueue(sensorData);
            }
            else
            {
                dataWindow.Dequeue();
                dataWindow.Enqueue(sensorData);
            }

            averageValue = Calculation.AverageQueue(dataWindow);
            IsMove = Calculation.IsMove(averageValue);
        }


        private void RotationToGameInput()
        {
            switch (direction)
            {
                case RotationDirection.XPOSITIVE:
                case RotationDirection.XNEGATIVE:
                    if (Mathf.Abs(averageValue.gyroX) > rotationThreshold)
                    {
                        value = averageValue.gyroX / maxRotation;
                    }
                    else value = 0;
                    break;
                
                case RotationDirection.YPOSITIVE:
                case RotationDirection.YNEGATIVE:
                    if (Mathf.Abs(averageValue.gyroY) > rotationThreshold)
                    {
                        value = averageValue.gyroY / maxRotation;
                    }
                    else value = 0;
                    break;
                
                case RotationDirection.ZPOSITIVE:
                case RotationDirection.ZNEGATIVE:
                    if (Mathf.Abs(averageValue.gyroZ) > rotationThreshold)
                    {
                        value = averageValue.gyroZ / maxRotation;
                    }
                    else value = 0;
                    break;
                
                default: break;
            }

        }


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
        
    }

}