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
        public MiniGame game;
        private SensorPairingData _sensorPairingData;
        private IDictionary<SensorPosition, SensorInput> sensorInputs = new Dictionary<SensorPosition, SensorInput>();
        private int sensorCount = 0;
        
        private void Awake()
        {
            LoadSensorData();

            switch (exercise)
            {
                case Exercise.Wheelchair:
                    sensorInputs.Add(SensorPosition.LEFT, new SensorInput(game, SensorPosition.LEFT, _sensorPairingData));
                    sensorInputs.Add(SensorPosition.RIGHT, new SensorInput(game, SensorPosition.RIGHT, _sensorPairingData));
                    SubscribeWheelchairEvent();
                    break;
                case Exercise.Dumbbell:
                    sensorInputs.Add(SensorPosition.LEFT, new SensorInput(game, SensorPosition.LEFT, _sensorPairingData));
                    sensorInputs.Add(SensorPosition.RIGHT, new SensorInput(game, SensorPosition.RIGHT, _sensorPairingData));
                    SubscribeDumbbellEvent2();
                    break;
                case Exercise.Cycle:
                    sensorInputs.Add(SensorPosition.LEFT, new SensorInput(game, SensorPosition.LEFT, _sensorPairingData));
                    SubscribeCycleEvent();
                    break;
                default:
                    break;
            }
            
            ConnectToSensors();
        }
        

        private void LoadSensorData()
        {
            _sensorPairingData = (SensorPairingData) DataSaver.LoadData(exercise.ToString() + ".sensorpair", typeof(SensorPairingData));
            if(_sensorPairingData == null) BackHome();
        }

        private void BackHome()
        {
            Debug.LogError("Please pair the sensors first."); //TODO
            SceneManager.LoadScene("Home");
        }

        public void SubscribeWheelchairEvent()
        {
            SyncsenseSensorManager.OnSensorDataReceivedEvent += sensorInputs[SensorPosition.LEFT].WheelchairControlEvent;
            SyncsenseSensorManager.OnSensorDataReceivedEvent += sensorInputs[SensorPosition.RIGHT].WheelchairControlEvent;
            Debug.Log("Haoming: Subscribe Success");
        }
        
        public void UnSubscribeWheelchairEvent()
        {
            SyncsenseSensorManager.OnSensorDataReceivedEvent -= sensorInputs[SensorPosition.LEFT].WheelchairControlEvent;
            SyncsenseSensorManager.OnSensorDataReceivedEvent -= sensorInputs[SensorPosition.RIGHT].WheelchairControlEvent;
        }
        
        public void SubscribeDumbbellEvent1()
        {
            SyncsenseSensorManager.OnSensorDataReceivedEvent += sensorInputs[SensorPosition.LEFT].DumbbellControlEvent1;
            SyncsenseSensorManager.OnSensorDataReceivedEvent += sensorInputs[SensorPosition.RIGHT].DumbbellControlEvent1;
        }
        
        public void UnSubscribeDumbbellEvent1()
        {
            SyncsenseSensorManager.OnSensorDataReceivedEvent -= sensorInputs[SensorPosition.LEFT].DumbbellControlEvent1;
            SyncsenseSensorManager.OnSensorDataReceivedEvent -= sensorInputs[SensorPosition.RIGHT].DumbbellControlEvent1;
        }
        
        public void SubscribeDumbbellEvent2()
        {
            SyncsenseSensorManager.OnSensorDataReceivedEvent += sensorInputs[SensorPosition.LEFT].DumbbellControlEvent2;
            SyncsenseSensorManager.OnSensorDataReceivedEvent += sensorInputs[SensorPosition.RIGHT].DumbbellControlEvent2;
        }
        
        public void UnSubscribeDumbbellEvent2()
        {
            SyncsenseSensorManager.OnSensorDataReceivedEvent -= sensorInputs[SensorPosition.LEFT].DumbbellControlEvent2;
            SyncsenseSensorManager.OnSensorDataReceivedEvent -= sensorInputs[SensorPosition.RIGHT].DumbbellControlEvent2;
        }
        
        public void SubscribeCycleEvent()
        {
            // Debug.Log("Haoming: Cycle Subscribe Success");
            SyncsenseSensorManager.OnSensorDataReceivedEvent += sensorInputs[SensorPosition.LEFT].CycleControlEvent;
        }
        
        public void UnSubscribeCycleEvent()
        {
            SyncsenseSensorManager.OnSensorDataReceivedEvent -= sensorInputs[SensorPosition.LEFT].CycleControlEvent;
        }
        
        public float GetData(SensorPosition position)
        {
            // Debug.Log("Haoming Value: " + gameInput[position].value);
            return UnityEngine.Random.Range(0, 1f);
            return sensorInputs[position].value;
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
            SyncsenseSensorManager.Instance.StopScan();
            
            SyncsenseSensorManager.Instance.DisconnectFromDevice(_sensorPairingData.leftSensorAddress);
            SyncsenseSensorManager.Instance.DisconnectFromDevice(_sensorPairingData.rightSensorAddress);
            SyncsenseSensorManager.OnScanResultEvent -= SensorManagerOnScanResultEvent;
            SyncsenseSensorManager.OnScanErrorEvent -= SensorManagerOnScanErrorEvent;

            SyncsenseSensorManager.OnDeviceConnectionStateChangeEvent -=
                SensorManagerOnDeviceConnectionStateChangeEvent;
            SyncsenseSensorManager.OnServicesDiscoveredEvent -= SensorManagerOnOnServicesDiscoveredEvent;
            
            UnSubscribeWheelchairEvent();
            UnSubscribeDumbbellEvent1();
            UnSubscribeDumbbellEvent2();
            UnSubscribeCycleEvent();
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