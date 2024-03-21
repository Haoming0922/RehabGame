using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.PXR;
using UnityEngine;

public class PairingSensorManager : MonoBehaviour
{
    [Header("UI Items")] 
    public GameObject manager;
    public TextMeshProUGUI textBluetoothEnabled;
    public TextMeshProUGUI textHasPermissions;
    public TextMeshProUGUI textConnectedDevices;

    private List<string> sensorList = new List<string>();
    // static Dictionary<string, SensorUpdater> sensorDict = new Dictionary<string, SensorUpdater>();
    
    private void OnDestroy()
    {
        DisConnect();
    }
    
    private void Start()
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
        
        // SyncsenseSensorManager.OnBatteryDataReceivedEvent += OnBatteryDataReceivedEvent;
        
        SyncsenseSensorManager.Instance.StartScan();
        Debug.Log("Start Scan");
        
        // PXR_Input.ResetController();
    }
    
    
    private void DisConnect()
    {
        foreach (var sensor in sensorList)
        {
            SyncsenseSensorManager.Instance.DisconnectFromDevice(sensor);
        }
        
        SyncsenseSensorManager.OnScanResultEvent -= SyncsenseSensorManagerOnOnScanResultEvent;
        SyncsenseSensorManager.OnScanErrorEvent -= SyncsenseSensorManagerOnOnScanErrorEvent;
        
        SyncsenseSensorManager.OnDeviceConnectionStateChangeEvent -= OnDeviceConnectionStateChangeEvent;
        SyncsenseSensorManager.OnServicesDiscoveredEvent -= OnOnServicesDiscoveredEvent;
    }
    
    
    private void SyncsenseSensorManagerOnOnScanErrorEvent(ScanError obj)
    {
        Debug.Log("Scan Error Code: " + obj.errorCode);
    }

    private void SyncsenseSensorManagerOnOnScanResultEvent(ScanResult obj)
    {
        Debug.Log("Scan Result: " + obj.name + " - " + obj.address);
        if (obj.name != null && obj.name.Equals("Cadence_Sensor"))
        {
            SyncsenseSensorManager.Instance.ConnectToDevice(obj.address);
        }
    }
    
    private void OnDeviceConnectionStateChangeEvent(ConnectionStateChange connectionStateChange)
    {
        if (connectionStateChange.newState == ConnectionState.STATE_CONNECTED)
        {
            if (sensorList.Contains(connectionStateChange.deviceAddress)) return; // we already have this device connected
            
            sensorList.Add(connectionStateChange.deviceAddress);
            textConnectedDevices.SetText("Connected devices: " + sensorList.Count);
            SyncsenseSensorManager.Instance.DiscoverServicesForDevice(connectionStateChange.deviceAddress);

        }
        if (connectionStateChange.newState == ConnectionState.STATE_DISCONNECTED)
        {
            sensorList.Remove(connectionStateChange.deviceAddress);
            textConnectedDevices.SetText("Connected devices: " + sensorList.Count);
            SyncsenseSensorManager.Instance.ConnectToDevice(connectionStateChange.deviceAddress);
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
        bool result = false;
        while (!result)
        {
            result = SyncsenseSensorManager.Instance.SubscribeToSensorData(discoveredServices.deviceAddress);
            if (!result) yield return new WaitForSeconds(1);
        }
        Debug.Log("[Haoming] SUBSCRIBED TO DEVICE: " + discoveredServices.deviceAddress);
        
        // result = false;
        // while (!result)
        // {
        //     result = SyncsenseSensorManager.Instance.SubscribeToBatteryData(discoveredServices.deviceAddress);
        //     
        //     if (!result) yield return new WaitForSeconds(1);
        // }
    }
    
    
    private void OnBatteryDataReceivedEvent(BatteryDataReceived data)
    {   
    }
    
    // private void HandleSensorData(SensorDataReceived data)
    // {   
    //     sensorDict[data.deviceAddress].data = data;
    //     sensorDict[data.deviceAddress].deviceAddress = data.deviceAddress;
    //     
    //     sensorDict[data.deviceAddress].samplesCounter ++;
    //
    //     // Check if one second has passed
    //     if (sensorDict[data.deviceAddress].stopwatch.ElapsedMilliseconds - sensorDict[data.deviceAddress].lastUpdateTime >= 1000)
    //     {
    //         sensorDict[data.deviceAddress].sampleRate = sensorDict[data.deviceAddress].samplesCounter;
    //         
    //         sensorDict[data.deviceAddress].samplesCounter = 0; // Reset counter
    //         sensorDict[data.deviceAddress].lastUpdateTime = sensorDict[data.deviceAddress].stopwatch.ElapsedMilliseconds; // Update the last update time
    //     }
    // }
    

}
