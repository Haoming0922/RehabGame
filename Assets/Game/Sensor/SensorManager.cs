using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.XR.PXR;
using UnityEngine.SceneManagement;

namespace Game.Sensor
{
    public class SensorManager : MonoBehaviour
    {
        public Exercise exercise;
        private SensorPairingData _sensorPairingData = new SensorPairingData();
        private IDictionary<SensorPosition, RotationController> gameInput = new Dictionary<SensorPosition, RotationController>();
        public int sensorCount = 0;
        public TextMeshProUGUI guide;
        
        private void Awake()
        {
            LoadConfigData();
        }

        private void LoadConfigData()
        {
            DataSaver.OnDataLoad += BackHome;
            _sensorPairingData = (SensorPairingData) DataSaver.LoadData(exercise.ToString() + ".sensorpair", typeof(SensorPairingData));
        }

        private void BackHome()
        {
            Debug.LogError("Please pair the sensors first."); //TODO
            SceneManager.LoadScene("Home");
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
        
        
        #region Dumbbell Calibrate
                
        public void OnDumbbellCalibrate()
        {
            StartCoroutine(DumbbellCalibrate());
        }

        public IEnumerator DumbbellCalibrate()
        {
            // transform.GetChild(0).gameObject.SetActive(true);
            // transform.GetChild(0).position = Camera.main.transform.position + 10 * Camera.main.transform.forward;
            
            guide.text = "Please move the sensors to wake them";
            yield return new WaitForSeconds(3f);

            UserConfig config = new UserConfig();

            yield return StartCoroutine(DumbbellCalibrateOneSide(SensorPosition.LEFT, config));
            yield return StartCoroutine(DumbbellCalibrateOneSide(SensorPosition.RIGHT, config));

            DataSaver.SaveData("Dumbbell.userconfig", config);

            transform.GetChild(0).gameObject.SetActive(false);
        }



        private IEnumerator DumbbellCalibrateOneSide(SensorPosition position, UserConfig config)
        {
            guide.text = "Please put down the " + position.ToString().ToLower() + " sensor";
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
                yield return null;
            }
            
            guide.text = "Please raise " + position.ToString().ToLower() + " side";
            yield return new WaitForSeconds(1f);

            float armRotationRaw = 0f;
            Quaternion armRotationMadgwick = Quaternion.identity;
            while (true)
            {
                float armRotationRawDelta = gameInput[position].averageValue.gyroY *
                                            Mathf.Sign(gameInput[position].averageValue.accX);

                armRotationMadgwick = Calculation.MadgwickIMU(gameInput[position].averageValue.gyroX,
                    gameInput[position].averageValue.gyroY,
                    gameInput[position].averageValue.gyroZ,
                    gameInput[position].averageValue.accX,
                    gameInput[position].averageValue.accY,
                    gameInput[position].averageValue.accZ,
                    Time.deltaTime,
                    armRotationMadgwick
                );

                if (armRotationRawDelta >= -10) armRotationRaw += armRotationRawDelta;
                else break;
            }

            switch (position)
            {
                case SensorPosition.LEFT:
                    config.maxLeftArmRotationAngle = Mathf.Abs(armRotationMadgwick.eulerAngles.x);
                    config.maxLeftArmRotationRaw = armRotationRaw;
                    break;
                case SensorPosition.RIGHT:
                    config.maxRightArmRotationAngle = Mathf.Abs(armRotationMadgwick.eulerAngles.x);
                    config.maxRightArmRotationRaw = armRotationRaw;
                    break;
            }

            yield return null;

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
            
            DataSaver.OnDataLoad -= BackHome;
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