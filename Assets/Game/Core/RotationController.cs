using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Core
{
    public class RotationController
    {
        public float value = 0;

        private string sensorAddress = "";

        private int lowPassWindowSize = 8;
        private SensorDataReceived averageValue;
        private float rotationThreshold = 20f;

        private Queue<SensorDataReceived> dataWindow = new Queue<SensorDataReceived>();
        
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
                    if (Mathf.Abs(averageValue.gyroX) > rotationThreshold)
                    {
                        value = averageValue.gyroX > 0 ? 1 : -1;
                    }
                    else value = 0;
                    break;
                case RotationDirection.XNEGATIVE:
                    if (Mathf.Abs(averageValue.gyroX) > rotationThreshold)
                    {
                        value = averageValue.gyroX > 0 ? -1 : 1;
                    }
                    else value = 0;
                    break;
                case RotationDirection.YPOSITIVE:
                    if (Mathf.Abs(averageValue.gyroY) > rotationThreshold)
                    {
                        value = averageValue.gyroY > 0 ? 1 : -1;
                    }
                    else value = 0;
                    break;
                case RotationDirection.YNEGATIVE:
                    if (Mathf.Abs(averageValue.gyroY) > rotationThreshold)
                    {
                        value = averageValue.gyroY > 0 ? -1 : 1;
                    }
                    else value = 0;
                    break;
                case RotationDirection.ZPOSITIVE:
                    if (Mathf.Abs(averageValue.gyroZ) > rotationThreshold)
                    {
                        value = averageValue.gyroX > 0 ? 1 : -1;
                    }
                    else value = 0;
                    break;
                case RotationDirection.ZNEGATIVE:
                    if (Mathf.Abs(averageValue.gyroZ) > rotationThreshold)
                    {
                        value = averageValue.gyroZ > 0 ? -1 : 1;
                    }
                    else value = 0;
                    break;
                default: break;
            }
        }


        public void SetCalibration()
        {
            if (Mathf.Abs(averageValue.gyroX) > Mathf.Abs(averageValue.gyroY))
            {
                if (Mathf.Abs(averageValue.gyroX) > Mathf.Abs(averageValue.gyroZ)) // X
                {
                    direction = averageValue.gyroX > 0 ? RotationDirection.XPOSITIVE :
                    RotationDirection.XNEGATIVE;
                }
                else // Z
                {
                    direction = averageValue.gyroZ > 0 ? RotationDirection.ZPOSITIVE :
                        RotationDirection.ZNEGATIVE;
                }
            }
            else
            {
                if (Mathf.Abs(averageValue.gyroY) > Mathf.Abs(averageValue.gyroZ)) // Y
                {
                    direction = averageValue.gyroY > 0 ? RotationDirection.YPOSITIVE :
                        RotationDirection.YNEGATIVE;
                }
                else // Z
                {
                    direction = averageValue.gyroZ > 0 ? RotationDirection.ZPOSITIVE :
                        RotationDirection.ZNEGATIVE;
                }
            }
        }
        
    }

}