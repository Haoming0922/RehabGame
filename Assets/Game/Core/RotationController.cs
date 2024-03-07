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
        private SensorDataReceived rotationValue;
        private float rotationThreshold = 20f;

        private RotationDirection direction;

        private Queue<SensorDataReceived> dataWindow = new Queue<SensorDataReceived>();


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

        public void SetRotationDirection(RotationDirection d)
        {
            direction = d;
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

            rotationValue = Calculation.AverageQueue(dataWindow);
        }


        private void RotationToGameInput()
        {
            switch (direction)
            {
                case RotationDirection.XPOSITIVE:
                    if (Mathf.Abs(rotationValue.gyroX) > rotationThreshold)
                    {
                        value = rotationValue.gyroX > 0 ? 1 : -1;
                    }
                    else value = 0;

                    break;
                case RotationDirection.XNEGATIVE:
                    if (Mathf.Abs(rotationValue.gyroX) > rotationThreshold)
                    {
                        value = rotationValue.gyroX > 0 ? -1 : 1;
                    }
                    else value = 0;

                    break;
                case RotationDirection.YPOSITIVE:
                    if (Mathf.Abs(rotationValue.gyroY) > rotationThreshold)
                    {
                        value = rotationValue.gyroY > 0 ? 1 : -1;
                    }
                    else value = 0;

                    break;
                case RotationDirection.YNEGATIVE:
                    if (Mathf.Abs(rotationValue.gyroY) > rotationThreshold)
                    {
                        value = rotationValue.gyroY > 0 ? -1 : 1;
                    }
                    else value = 0;

                    break;
                case RotationDirection.ZPOSITIVE:
                    if (Mathf.Abs(rotationValue.gyroZ) > rotationThreshold)
                    {
                        value = rotationValue.gyroX > 0 ? 1 : -1;
                    }
                    else value = 0;

                    break;
                case RotationDirection.ZNEGATIVE:
                    if (Mathf.Abs(rotationValue.gyroZ) > rotationThreshold)
                    {
                        value = rotationValue.gyroZ > 0 ? -1 : 1;
                    }
                    else value = 0;

                    break;
            }
        }

    }

}