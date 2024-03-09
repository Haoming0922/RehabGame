using System;
using System.Collections;
using System.Collections.Generic;
using Game.JumpJump;
using TMPro;
using UnityEngine;
using Unity.XR.PXR;

namespace Game.Core
{
    public class SensorManager : MonoBehaviour
    {
        public Exercise exercise;
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
            transform.GetChild(0).position = Camera.main.transform.position + 30 * Camera.main.transform.forward;

            guide.text = "Connecting to sensors...";
            
            yield return new WaitForSeconds(0.5f);
            
            while (sensorCount < gameInput.Count)
            {
                guide.text = "Please move the sensors to wake them";
                yield return null;
            }

            bool leftCalibrated = false;
            bool rightCalibrated = false;
            while (!leftCalibrated)
            {
                guide.text = "Please hold the left sensor still";
                yield return new WaitForSeconds(0.5f);
                if (!gameInput[SensorPosition.LEFT].IsMove)
                {
                    guide.text = "Please raise left side";
                    yield return new WaitForSeconds(0.1f);
                    
                    while (true)
                    {
                        if (gameInput[SensorPosition.LEFT].IsMove)
                        {
                            gameInput[SensorPosition.LEFT].SetCalibration();
                            leftCalibrated = true;
                            break;
                        }
                        yield return null;
                    }
                }
            }
            while (!rightCalibrated)
            {
                guide.text = "Please hold the right sensor still";
                yield return new WaitForSeconds(0.5f);
                if (!gameInput[SensorPosition.RIGHT].IsMove)
                {
                    guide.text = "Please raise right side";
                    yield return new WaitForSeconds(0.1f);
                    
                    while (true)
                    {
                        if (gameInput[SensorPosition.RIGHT].IsMove)
                        {
                            gameInput[SensorPosition.RIGHT].SetCalibration();
                            rightCalibrated = true;
                            break;
                        }
                        yield return null;
                    }
                }
            }
            
            transform.GetChild(0).gameObject.SetActive(false);
            
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