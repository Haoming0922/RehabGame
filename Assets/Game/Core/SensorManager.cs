using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.XR.PXR;

namespace Game.Core
{
    public class SensorManager : MonoBehaviour
    {
        public Exercise exercise;
        public ClassicProgressBar progressBar;
        public TextMeshProUGUI guide;
        
        private SensorPairingData _sensorPairingData = new SensorPairingData();
        private IDictionary<SensorPosition, RotationController> gameInput =
            new Dictionary<SensorPosition, RotationController>();
        private int sensorCount = 0;
        
        private void Awake()
        {
            _sensorPairingData.LoadData(exercise);
        }

        private void Start()
        {
            gameInput.Add(SensorPosition.LEFT, new RotationController(SensorPosition.LEFT, _sensorPairingData));
            gameInput.Add(SensorPosition.RIGHT, new RotationController(SensorPosition.RIGHT, _sensorPairingData));
            ConnectToSensors();
            progressBar.m_FillAmount = 0;
        }

        public float GetData(SensorPosition position)
        {
            return gameInput[position].value;
        }
        
        
        #region Dumbbell Calibrate Event
        
        public void OnDumbbellCalibrate()
        {
            StartCoroutine(DumbbellCalibrate());
        }
        
        
        public IEnumerator DumbbellCalibrate()
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(0).position = Camera.main.transform.position + 10 * Camera.main.transform.forward;

            guide.text = "Connecting to sensors...";
            
            yield return new WaitForSeconds(1f);
            
            while (sensorCount < gameInput.Count)
            {
                guide.text = "Please move the sensors to wake them";
                yield return null;
            }

            yield return StartCoroutine(DumbbellCalibrateOneSide(SensorPosition.LEFT));
            yield return StartCoroutine(DumbbellCalibrateOneSide(SensorPosition.RIGHT));
            
            transform.GetChild(0).gameObject.SetActive(false);
        }



        private IEnumerator DumbbellCalibrateOneSide(SensorPosition position)
        {
            progressBar.m_FillAmount = 0;
            guide.text = "Please hold the " + position.ToString().ToLower() +  " sensor at start position";
            yield return new WaitForSeconds(1f);
            float t = 0f;
            float waitTime = 5f;
            while (t < waitTime)
            {
                if (!gameInput[position].IsMove)
                {
                    t += Time.deltaTime;
                }
                else t = 0;
                progressBar.m_FillAmount = t / waitTime;
                yield return null;
            }

            progressBar.m_FillAmount = 0;
            guide.text = "Please raise " + position.ToString().ToLower() + " side as far as you can";
            yield return new WaitForSeconds(1f);
            SensorDataReceived rotationAmount = new SensorDataReceived();
            float maxRotation = 0f;
            
            bool calibrated = false;
            while (!calibrated)
            {
                if (gameInput[position].IsMove)
                {
                    rotationAmount += gameInput[position].averageValue;
                    gameInput[position].SetCalibration(rotationAmount);
                    
                    switch (gameInput[position].direction)
                    {
                        case RotationDirection.XPOSITIVE:
                        case RotationDirection.XNEGATIVE:
                            if (Mathf.Abs(rotationAmount.gyroX) > Mathf.Abs(maxRotation))
                            {
                                maxRotation = rotationAmount.gyroX;
                                gameInput[position].SetCalibration(rotationAmount);
                            }
                            else
                            {
                                calibrated = true;
                            }
                            break;
                        case RotationDirection.YPOSITIVE:
                        case RotationDirection.YNEGATIVE:
                            if (Mathf.Abs(rotationAmount.gyroY) > Mathf.Abs(maxRotation))
                            {
                                maxRotation = rotationAmount.gyroY;
                                gameInput[position].SetCalibration(rotationAmount);
                            }
                            else
                            {
                                calibrated = true;
                            }
                            break;
                        case RotationDirection.ZPOSITIVE:
                        case RotationDirection.ZNEGATIVE:
                            if (Mathf.Abs(rotationAmount.gyroZ) > Mathf.Abs(maxRotation))
                            {
                                maxRotation = rotationAmount.gyroZ;
                                gameInput[position].SetCalibration(rotationAmount);
                            }
                            else
                            {
                                calibrated = true;
                            }
                            break;
                    }
                }
                
                progressBar.m_FillAmount = maxRotation / 100f;
                yield return null;
            }
        }
        
        #endregion
        
        #region Connect to Sensors
        

        public void ConnectToSensors()
        {
            if (!SyncsenseSensorManager.Instance.IsBluetoothEnabled())
            {
                SyncsenseSensorManager.Instance.RequestBluetoothEnable();
            }

            if (!SyncsenseSensorManager.Instance.HasPermissions())
            {
                SyncsenseSensorManager.Instance.RequestPermissions();
            }

            SyncsenseSensorManager.OnScanResultEvent += SensorManagerOnScanResultEvent;
            SyncsenseSensorManager.OnScanErrorEvent += SensorManagerOnScanErrorEvent;

            SyncsenseSensorManager.OnDeviceConnectionStateChangeEvent +=
                SensorManagerOnDeviceConnectionStateChangeEvent;
            SyncsenseSensorManager.OnServicesDiscoveredEvent += SensorManagerOnOnServicesDiscoveredEvent;
            
            SyncsenseSensorManager.Instance.StartScan();
            Debug.Log("Start Scan");
        
            PXR_Input.ResetController();
        }

        public void OnDestroy()
        {
            SyncsenseSensorManager.Instance.DisconnectFromDevice(_sensorPairingData.leftSensorAddress);
            SyncsenseSensorManager.Instance.DisconnectFromDevice(_sensorPairingData.rightSensorAddress);

            SyncsenseSensorManager.OnScanResultEvent += SensorManagerOnScanResultEvent;
            SyncsenseSensorManager.OnScanErrorEvent += SensorManagerOnScanErrorEvent;

            SyncsenseSensorManager.OnDeviceConnectionStateChangeEvent +=
                SensorManagerOnDeviceConnectionStateChangeEvent;
            SyncsenseSensorManager.OnServicesDiscoveredEvent += SensorManagerOnOnServicesDiscoveredEvent;
        }

        private void SensorManagerOnScanErrorEvent(ScanError obj)
        {
            Debug.Log("Scan Error Code: " + obj.errorCode);
        }

        private void SensorManagerOnScanResultEvent(ScanResult obj)
        {
            Debug.Log("Scan Result: " + obj.name + " - " + obj.address);
            if (obj.name != null && obj.name.Equals("Cadence_Sensor"))
            {
                if (obj.address == _sensorPairingData.leftSensorAddress ||
                    obj.address == _sensorPairingData.rightSensorAddress)
                {
                    SyncsenseSensorManager.Instance.ConnectToDevice(obj.address);
                }
            }
        }

        private void SensorManagerOnDeviceConnectionStateChangeEvent(ConnectionStateChange connectionStateChange)
        {
            if (connectionStateChange.newState == ConnectionState.STATE_CONNECTED)
            {
                SyncsenseSensorManager.Instance.DiscoverServicesForDevice(connectionStateChange.deviceAddress);
            }

            if (connectionStateChange.newState == ConnectionState.STATE_DISCONNECTED)
            {
                SyncsenseSensorManager.Instance.ConnectToDevice(connectionStateChange.deviceAddress);
            }
        }

        private void SensorManagerOnOnServicesDiscoveredEvent(ServicesDiscovered discoveredServices)
        {
            foreach (ServiceItem serviceItem in discoveredServices.services)
            {
                Debug.Log("Found Service: " + serviceItem.serviceUuid);
                foreach (CharacteristicItem characteristicItem in serviceItem.characteristics)
                {
                    Debug.Log(" - Found Characteristic: " + characteristicItem.characteristicUuid);
                }
            }

            // Subscription can fail at the enabling level. In that scenario, the subscription attempt must be retried.
            StartCoroutine(attemptToSubscribe(discoveredServices));
        }

        private IEnumerator attemptToSubscribe(ServicesDiscovered discoveredServices)
        {
            bool result = false;
            while (!result)
            {
                result = SyncsenseSensorManager.Instance.SubscribeToSensorData(discoveredServices.deviceAddress);
                if (!result) yield return new WaitForSeconds(1);
            }

            Debug.Log("SUBSCRIBED TO DEVICE: " + discoveredServices.deviceAddress);
            sensorCount++;
        }

        #endregion
        
    }
}