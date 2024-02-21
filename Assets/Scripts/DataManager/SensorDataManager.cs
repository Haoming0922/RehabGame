using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.PXR;
using UnityEngine;

public class SensorDataManager : Singleton<SensorDataManager>
{
    // ------------ General UI ------------
    [Space(10)]
    [Header("General UI Items")]
    public TextMeshProUGUI textBluetoothEnabled;
    public TextMeshProUGUI textHasPermissions;
    public TextMeshProUGUI textConnectedDevices;

    public bool useSensorFusion = false;
    
    static int connectedDevices = 0;

    static Dictionary<string, SensorUpdater> sensorDict = new Dictionary<string, SensorUpdater>();
    
    void Start()
    {
        if (!SyncsenseSensorManager.Instance.IsBluetoothEnabled())
        {
            SyncsenseSensorManager.Instance.RequestBluetoothEnable();
        }
        
        if (!SyncsenseSensorManager.Instance.HasPermissions())
        {
            SyncsenseSensorManager.Instance.RequestPermissions();
        }
        
        textBluetoothEnabled.SetText(SyncsenseSensorManager.Instance.IsBluetoothEnabled()?"BL is Enabled":"BL is Disabled");
        textHasPermissions.SetText(SyncsenseSensorManager.Instance.HasPermissions()?"We have Permissions":"We don't have Permissions");
        
        SyncsenseSensorManager.OnScanResultEvent += SyncsenseSensorManagerOnOnScanResultEvent;
        SyncsenseSensorManager.OnScanErrorEvent += SyncsenseSensorManagerOnOnScanErrorEvent;
        
        SyncsenseSensorManager.OnDeviceConnectionStateChangeEvent += OnDeviceConnectionStateChangeEvent;
        SyncsenseSensorManager.OnServicesDiscoveredEvent += OnOnServicesDiscoveredEvent;
        
        // SyncsenseSensorManager.OnSensorDataReceivedEvent += OnSensorDataReceivedEvent;
        // SyncsenseSensorManager.OnBatteryDataReceivedEvent += OnBatteryDataReceivedEvent;
        
        SyncsenseSensorManager.Instance.StartScan();
        
        SyncsenseSensorManager.Instance.EnableWriteToFile(true);
        
        PXR_Input.ResetController();
    }

    private void OnDestroy()
    {
        SyncsenseSensorManager.OnScanResultEvent -= SyncsenseSensorManagerOnOnScanResultEvent;
        SyncsenseSensorManager.OnScanErrorEvent -= SyncsenseSensorManagerOnOnScanErrorEvent;
        
        SyncsenseSensorManager.OnDeviceConnectionStateChangeEvent -= OnDeviceConnectionStateChangeEvent;
        SyncsenseSensorManager.OnServicesDiscoveredEvent -= OnOnServicesDiscoveredEvent;
        
        // SyncsenseSensorManager.OnSensorDataReceivedEvent -= OnSensorDataReceivedEvent;
        // SyncsenseSensorManager.OnBatteryDataReceivedEvent -= OnBatteryDataReceivedEvent;
    }





    private void SyncsenseSensorManagerOnOnScanErrorEvent(ScanError obj)
    {
        Debug.Log("Scan Error Code: " + obj.errorCode);
    }

    private void SyncsenseSensorManagerOnOnScanResultEvent(ScanResult obj)
    {
        Debug.Log("Scan Result: " + obj.name + " - " + obj.address);
        if (obj.name != null && obj.name.Equals("Cadence_Sensor") && connectedDevices < 10)
        {
            SyncsenseSensorManager.Instance.ConnectToDevice(obj.address);
        }
    }
    
    private void OnDeviceConnectionStateChangeEvent(ConnectionStateChange connectionStateChange)
    {
        if (connectionStateChange.newState == ConnectionState.STATE_CONNECTED)
        {
            if (sensorDict.ContainsKey(connectionStateChange.deviceAddress))
            {
                // we already have this device connected
                return;
            }
            
            connectedDevices++;
            textConnectedDevices.SetText("Connected devices: " + connectedDevices);
            SyncsenseSensorManager.Instance.DiscoverServicesForDevice(connectionStateChange.deviceAddress);

            // if (_connectedDevices == 2) SyncsenseSensorManager.Instance.StopScan();
        }
        if (connectionStateChange.newState == ConnectionState.STATE_DISCONNECTED)
        {
            connectedDevices--;
            textConnectedDevices.SetText("Connected devices: " + connectedDevices);

            sensorDict.Remove(connectionStateChange.deviceAddress);
            
            if (GameDataManager.Instance.sensorMapping.ContainsKey(connectionStateChange.deviceAddress))
            {
                throw new Exception("Connection Lost.");
            }
        }
    }
    
    private void OnOnServicesDiscoveredEvent(ServicesDiscovered discoveredServices)
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
        sensorDict.Add(discoveredServices.deviceAddress, new SensorUpdater());

        bool result = false;
        
        while (!result)
        {
            result = SyncsenseSensorManager.Instance.SubscribeToSensorData(discoveredServices.deviceAddress);
            
            if (!result) yield return new WaitForSeconds(1);
        }
        
        // result = false;
        // while (!result)
        // {
        //     result = SyncsenseSensorManager.Instance.SubscribeToBatteryData(discoveredServices.deviceAddress);
        //     
        //     if (!result) yield return new WaitForSeconds(1);
        // }

        Debug.Log("SUBSCRIBED TO DEVICE: " + discoveredServices.deviceAddress);
    }

    private void OnSensorDataReceivedEvent(SensorDataReceived data)
    {
        HandleSensorData(data);
    }
    
    // private void OnBatteryDataReceivedEvent(BatteryDataReceived data)
    // {   
    //     _deviceMessageUIUpdaters[data.deviceAddress]?.HandleBatteryData(data);
    // }
    
    
    private void HandleSensorData(SensorDataReceived data)
    {   

        sensorDict[data.deviceAddress].data = data;
        sensorDict[data.deviceAddress].deviceAddress = data.deviceAddress;
        
        sensorDict[data.deviceAddress].samplesCounter ++;

        // Check if one second has passed
        if (sensorDict[data.deviceAddress].stopwatch.ElapsedMilliseconds - sensorDict[data.deviceAddress].lastUpdateTime >= 1000)
        {
            sensorDict[data.deviceAddress].sampleRate = sensorDict[data.deviceAddress].samplesCounter;
            
            sensorDict[data.deviceAddress].samplesCounter = 0; // Reset counter
            sensorDict[data.deviceAddress].lastUpdateTime = sensorDict[data.deviceAddress].stopwatch.ElapsedMilliseconds; // Update the last update time
        }
    }


    

}
