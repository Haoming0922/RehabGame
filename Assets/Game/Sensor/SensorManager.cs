using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.XR.PXR;
using UnityEngine.SceneManagement;
using Game.Util;

namespace Game.Sensor
{
    public class SensorManager : MonoBehaviour
    {
        public Exercise exercise;
        private SensorPairingData _sensorPairingData = new SensorPairingData();
        private UserConfig _userConfig = new UserConfig();
        private IDictionary<SensorPosition, RotationController> gameInput = new Dictionary<SensorPosition, RotationController>();
        private int sensorCount = 0;
        
        private void Awake()
        {
            LoadConfigData();
        }

        private void LoadConfigData()
        {
            _sensorPairingData = (SensorPairingData) DataSaver.LoadData(exercise.ToString() + ".sensorpair", typeof(SensorPairingData));
            if(_sensorPairingData == null) BackHome();
            
            _userConfig = (UserConfig) DataSaver.LoadData(exercise.ToString() + ".userconfig", typeof(UserConfig));
            if(_userConfig == null && exercise == Exercise.DUMBBELL) BackHome();
        }

        private void BackHome()
        {
            Debug.LogError("Please pair the sensors first."); //TODO
            SceneManager.LoadScene("Home");
        }
        
        private void Start()
        {
            gameInput.Add(SensorPosition.LEFT, new RotationController(SensorPosition.LEFT, _sensorPairingData, _userConfig));
            gameInput.Add(SensorPosition.RIGHT, new RotationController(SensorPosition.RIGHT, _sensorPairingData, _userConfig));
            ConnectToSensors();
        }

        public void SubscribeWheelchairEvent()
        {
            SyncsenseSensorManager.OnSensorDataReceivedEvent += gameInput[SensorPosition.LEFT].WheelchairControlEvent;
            SyncsenseSensorManager.OnSensorDataReceivedEvent += gameInput[SensorPosition.RIGHT].WheelchairControlEvent;
        }
        
        public void UnSubscribeWheelchairEvent()
        {
            SyncsenseSensorManager.OnSensorDataReceivedEvent -= gameInput[SensorPosition.LEFT].WheelchairControlEvent;
            SyncsenseSensorManager.OnSensorDataReceivedEvent -= gameInput[SensorPosition.RIGHT].WheelchairControlEvent;
        }
        
        public void SubscribeDumbbellEvent1()
        {
            SyncsenseSensorManager.OnSensorDataReceivedEvent += gameInput[SensorPosition.LEFT].DumbbellControlEvent1;
            SyncsenseSensorManager.OnSensorDataReceivedEvent += gameInput[SensorPosition.RIGHT].DumbbellControlEvent1;
        }
        
        public void UnSubscribeDumbbellEvent1()
        {
            SyncsenseSensorManager.OnSensorDataReceivedEvent -= gameInput[SensorPosition.LEFT].DumbbellControlEvent1;
            SyncsenseSensorManager.OnSensorDataReceivedEvent -= gameInput[SensorPosition.RIGHT].DumbbellControlEvent1;
        }
        
        public void SubscribeDumbbellEvent2()
        {
            SyncsenseSensorManager.OnSensorDataReceivedEvent += gameInput[SensorPosition.LEFT].DumbbellControlEvent2;
            SyncsenseSensorManager.OnSensorDataReceivedEvent += gameInput[SensorPosition.RIGHT].DumbbellControlEvent2;
        }
        
        public void UnSubscribeDumbbellEvent2()
        {
            SyncsenseSensorManager.OnSensorDataReceivedEvent -= gameInput[SensorPosition.LEFT].DumbbellControlEvent2;
            SyncsenseSensorManager.OnSensorDataReceivedEvent -= gameInput[SensorPosition.RIGHT].DumbbellControlEvent2;
        }
        
        public float GetData(SensorPosition position)
        {
            return gameInput[position].value;
        }
        
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