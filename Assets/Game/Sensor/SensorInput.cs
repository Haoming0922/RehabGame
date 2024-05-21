using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Game.Util;
using RehabDB;

namespace Game.Sensor
{
    public class SensorInput
    {
        public float value;
        public float baseValue = 0;
        
        private string sensorAddress = "";
        private float currentRotationRaw = 0;
        private RotationDirection direction = RotationDirection.NULL;

        private Queue<float> dataWindow = new Queue<float>();
        private int lowPassWindowSize = 6;
        public float averageValue { get; private set; }
        public bool IsMove { get; private set; }
        public bool IsDown { get; private set; }

        private float angleStart = 1f;
        private float angleEnd = 1f;

        private float CFWeightAcc = 0.14f;
        private float gravity = 9.8f;
        
        public SensorInput(MiniGame game, SensorPosition position, SensorPairingData pairingData, LocalPatientData localPatient)
        {
            switch (position)
            {
                case SensorPosition.LEFT:
                    direction = pairingData.leftSensorDirection;
                    sensorAddress = pairingData.leftSensorAddress;
                    gravity = pairingData.leftSensorGravity;
                    // Debug.Log("Haoming: " + pairingData.leftSensorAddress);
                    LoadDefaultBaseValue(game, SensorPosition.LEFT, localPatient);
                    
                    break;
                case SensorPosition.RIGHT:
                    direction = pairingData.rightSensorDirection;
                    sensorAddress = pairingData.rightSensorAddress;
                    gravity = pairingData.rightSensorGravity;
                    LoadDefaultBaseValue(game, SensorPosition.RIGHT, localPatient); // TODO: Fetch base value from DB
                    break;
            }
            
        }

        void LoadDefaultBaseValue(MiniGame game, SensorPosition position, LocalPatientData localPatient)
        {
            switch (game)
            {
                case MiniGame.JumpJump:
                    baseValue = 50f;
                    // baseValue = localPatient.JumpJumpPerformance;
                    break;
                case MiniGame.WheelChair:
                    baseValue = 50f;
                    // baseValue = localPatient.WheelchairPerformanceLeft;
                    break;
                case MiniGame.Cycle:
                    baseValue = 40f;
                    // baseValue = localPatient.CyclePerformance;
                    break;
            }
        }

        void SetDefaultBaseValue(MiniGame game, SensorPosition position)
        {
            if (DBManager.Instance.currentPatient == null)
            {
                if (game == MiniGame.JumpJump) baseValue = 50f;
                if (game == MiniGame.WheelChair) baseValue = 50f;
                if (game == MiniGame.Cycle) baseValue = 100f;
            }
            else
            {
                if (position == SensorPosition.LEFT)
                {
                    if (game == MiniGame.JumpJump) baseValue = DBManager.Instance.currentPatient.FindGamePerformance("Jump Jump") == null ? 50f : DBManager.Instance.currentPatient.FindGamePerformance("Jump Jump").leftInputPerformance;
                    if (game == MiniGame.WheelChair) baseValue = DBManager.Instance.currentPatient.FindGamePerformance("WheelChair") == null ? 50f : DBManager.Instance.currentPatient.FindGamePerformance("WheelChair").leftInputPerformance;
                    if (game == MiniGame.Cycle) baseValue = DBManager.Instance.currentPatient.FindGamePerformance("Cycle") == null ? 100f : DBManager.Instance.currentPatient.FindGamePerformance("Cycle").leftInputPerformance;
                }
                else if (position == SensorPosition.RIGHT)
                {
                    if (game == MiniGame.JumpJump) baseValue = DBManager.Instance.currentPatient.FindGamePerformance("Jump Jump") == null ? 50f : DBManager.Instance.currentPatient.FindGamePerformance("Jump Jump").rightInputPerformance;
                    if (game == MiniGame.WheelChair) baseValue = DBManager.Instance.currentPatient.FindGamePerformance("WheelChair") == null ? 50f : DBManager.Instance.currentPatient.FindGamePerformance("WheelChair").rightInputPerformance;
                    if (game == MiniGame.Cycle) baseValue = DBManager.Instance.currentPatient.FindGamePerformance("Cycle") == null ? 100f : DBManager.Instance.currentPatient.FindGamePerformance("Cycle").rightInputPerformance;
                }
            }
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

        public void DumbbellControlEvent(SensorDataReceived sensorData)
        {
            if (sensorAddress == sensorData.deviceAddress)
            {
                MotionLowPassFiltRotation(sensorData);
                MotionToGameInput();
            }
        }
        
        public void MotionToGameInput()
        {
            if (value > 1000)
            {
                ResetValue();
            }
            else
            {
                value += averageValue > 15 ? averageValue : 0;
            }
        }

        public void ResetValue()
        {
            dataWindow.Clear();
            averageValue = 0;
            value = 0;
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
            int sign = 1;
            
            if (Calculation.IsDownX(sensorData) && !Calculation.IsMove(sensorData)){
                dataWindow.Clear();
                
                sign = sensorData.accX > 0 ? 1 : -1;
                
                angleStart = 0;
                angleEnd = angleStart;
            }
            
            angleEnd = Calculation.ComplementaryFilterRotationY(sensorData.gyroY, sensorData.accX, angleEnd, CFWeightAcc, sign * gravity);
            
            // angleEnd = Calculation.ComplementaryFilterRotationY(sensorData.gyroY, sensorData.accX, angleEnd, CFWeightAcc, gravity);
        
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
            if (averageValue > 10)
            {
                averageValue = Mathf.Clamp(averageValue, 0, 150);
                value = averageValue / baseValue;
            }
            else
            {
                value = 0;
            }
        }
        
        private void DumbbellRotationToGameInput2(SensorDataReceived sensorData) //JumpJump
        {
            float rotate = sensorData.gyroY * Mathf.Sign(sensorData.accX);
            if (rotate > 10f) // up
            {
                averageValue = Mathf.Clamp(averageValue, 0, 150);
                value = averageValue / baseValue;
            }
            else if (rotate < -10f) // down
            {
                value = -1;
            }
            else
            {
                value = 0;
            }
        }
        
        
        private void MotionLowPassFiltRotation(SensorDataReceived sensorData)
        {
            if (dataWindow.Count < lowPassWindowSize)
            {
                dataWindow.Enqueue(Calculation.AccMotion(sensorData));
            }
            else
            {
                dataWindow.Dequeue();
                dataWindow.Enqueue(Calculation.AccMotion(sensorData));
            }
            
            averageValue = Calculation.AverageQueue(dataWindow);
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
                        value = Calculation.ToWheelchairRacingInput(Mathf.Abs(averageValue), baseValue);
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
                        value = Calculation.ToWheelchairRacingInput(Mathf.Abs(averageValue), baseValue);
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
                // Debug.Log("Haoming: " + averageValue);
                averageValue = Calculation.AccMotion(sensorData);
                value = Calculation.ToCycleRacingInput(averageValue, baseValue);
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