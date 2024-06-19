using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Util;
using TMPro;
using Unity.XR.PXR;
using UnityEngine.SceneManagement;

namespace Game.Sensor
{


    public class ROMMeasurement : MonoBehaviour
    {
            public TextMeshProUGUI guide;
            public PairingAvatar avatar;
            public float CFWeightAcc;

            private string currentSensorAddress = string.Empty;
            private string targetSensorAddress = string.Empty;
            private SensorPosition currentPairingPosition = SensorPosition.LEFT;
            private float rotaionYMax = 0;
            private float rotaionYStart = 0;
            private float rotaionYEnd = 0;
            private Dictionary<string, float> restGravity = new Dictionary<string, float>();
            private Queue<SensorDataReceived> dataQueue = new Queue<SensorDataReceived>();
            private int windowSize = 8;

            private bool isDown = false;


            private SensorPairingData _sensorPairingData;

            private void Start()
            {
                LoadSensorData();
                ConnectToSensors();
                StartDumbbellPair();
            }

            public void StartDumbbellPair()
            {
                StartCoroutine(DumbbellPairCalibrate());
            }


            private void LoadSensorData()
            {
                _sensorPairingData =
                    (SensorPairingData)DataSaver.LoadData("Dumbbell.sensorpair", typeof(SensorPairingData));
                if (_sensorPairingData == null) BackHome();
            }

            private void BackHome()
            {
                Debug.LogError("Please pair the sensors first."); //TODO
                SceneManager.LoadScene("Home");
            }

            private void OnDisable()
            {
                SyncsenseSensorManager.OnSensorDataReceivedEvent -= DumbbellPairEvent;
            }

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

            private void OnDestroy()
            {
                SyncsenseSensorManager.OnSensorDataReceivedEvent -= DumbbellPairEvent;

                SyncsenseSensorManager.Instance.StopScan();

                SyncsenseSensorManager.Instance.DisconnectFromDevice(_sensorPairingData.leftSensorAddress);
                SyncsenseSensorManager.Instance.DisconnectFromDevice(_sensorPairingData.rightSensorAddress);
                SyncsenseSensorManager.OnScanResultEvent -= SensorManagerOnScanResultEvent;
                SyncsenseSensorManager.OnScanErrorEvent -= SensorManagerOnScanErrorEvent;

                SyncsenseSensorManager.OnDeviceConnectionStateChangeEvent -=
                    SensorManagerOnDeviceConnectionStateChangeEvent;
                SyncsenseSensorManager.OnServicesDiscoveredEvent -= SensorManagerOnOnServicesDiscoveredEvent;

            }


            private void DumbbellPairEvent(SensorDataReceived data)
            {
                // select the first moving sensor and initialize
                if (currentSensorAddress == string.Empty && targetSensorAddress == data.deviceAddress)
                {
                    currentSensorAddress = data.deviceAddress;
                    rotaionYMax = 0;

                    dataQueue.Clear();

                    float ax = data.accX;
                    if (ax < -restGravity[currentSensorAddress]) ax = -restGravity[currentSensorAddress];
                    else if (ax > restGravity[currentSensorAddress]) ax = restGravity[currentSensorAddress];

                    rotaionYStart = Mathf.Acos(ax / restGravity[currentSensorAddress]) * Mathf.Rad2Deg;

                    if (rotaionYStart > 90f) // X axis points upwards
                    {
                        restGravity[currentSensorAddress] *= -1;
                    }

                    rotaionYStart = 0f;
                    rotaionYEnd = rotaionYStart;
                }

                // update angle
                if (currentSensorAddress == data.deviceAddress)
                {
                    SensorDataReceived filterData = LowPassFilter(data);
                    rotaionYEnd = Calculation.ComplementaryFilterRotationY(filterData.gyroY, filterData.accX,
                        rotaionYEnd, CFWeightAcc, restGravity[currentSensorAddress]);
                    // Debug.Log("[Haoming] rotaionYEnd: "½ + rotaionYEnd + ", rotaionYStart: " + rotaionYStart);
                    avatar.UpdateArmRotation(currentPairingPosition, rotaionYEnd - rotaionYStart);
                }

                // update max angle
                if (Mathf.Abs(rotaionYEnd - rotaionYStart) > rotaionYMax)
                {
                    rotaionYMax = Mathf.Abs(rotaionYEnd - rotaionYStart);
                }

            }



            private SensorDataReceived LowPassFilter(SensorDataReceived data)
            {
                if (dataQueue.Count < windowSize)
                {
                    dataQueue.Enqueue(data);
                }
                else
                {
                    dataQueue.Dequeue();
                    dataQueue.Enqueue(data);
                }

                return Calculation.AverageQueue(dataQueue);
            }


            public IEnumerator DumbbellPairCalibrate()
            {
                guide.text = "Please move the sensors to wake them";
                yield return new WaitForSeconds(3f);

                guide.text = "Please hold sensors still and DO NOT move";
                yield return new WaitForSeconds(3f);
                yield return StartCoroutine(DumbbellPairCalibrateRest());

                targetSensorAddress = _sensorPairingData.leftSensorAddress;
                yield return StartCoroutine(DumbbellPairCalibrateOneSide(SensorPosition.LEFT));
                targetSensorAddress = _sensorPairingData.rightSensorAddress;
                yield return StartCoroutine(DumbbellPairCalibrateOneSide(SensorPosition.RIGHT));

                BackHome();
            }



            private void DumbbellPairRestEvent(SensorDataReceived data)
            {
                if (!restGravity.TryAdd(data.deviceAddress, Calculation.GetRestGravity(data)))
                {
                    restGravity[data.deviceAddress] =
                        (restGravity[data.deviceAddress] + Calculation.GetRestGravity(data)) / 2f;
                }
            }


            private IEnumerator DumbbellPairCalibrateRest()
            {
                SyncsenseSensorManager.OnSensorDataReceivedEvent += DumbbellPairRestEvent;

                float t = 0f;
                while (t < 5f)
                {
                    t += Time.deltaTime;
                    yield return null;
                }

                SyncsenseSensorManager.OnSensorDataReceivedEvent -= DumbbellPairRestEvent;

            }



            private IEnumerator DumbbellPairCalibrateOneSide(SensorPosition position)
            {
                currentPairingPosition = position;

                guide.text = "Please put down the sensors";

                avatar.FindArm();
                yield return StartCoroutine(avatar.PutArmDown());

                yield return new WaitForSeconds(2f);

                // float t = 0f;
                // float waitTime = 3f;
                // while (t < waitTime)
                // {
                //     if (isDown)
                //     {
                //         t += Time.deltaTime;
                //     }
                //     else t = 0;
                //     yield return null;
                // }

                SyncsenseSensorManager.OnSensorDataReceivedEvent -= DumbbellPairEvent;
                SyncsenseSensorManager.OnSensorDataReceivedEvent += DumbbellPairEvent;

                guide.text = "Please only raise " + currentPairingPosition.ToString().ToLower() + " side";
                yield return new WaitForSeconds(8f);
                guide.text = "Max Angle: " + (int)rotaionYMax;
                yield return new WaitForSeconds(3f);

                SyncsenseSensorManager.OnSensorDataReceivedEvent -= DumbbellPairEvent;

                Reset();
            }

            private void Reset()
            {
                currentSensorAddress = string.Empty;
                rotaionYMax = 0;
                rotaionYStart = 0;
                rotaionYEnd = 0;
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
            }


        }
    }