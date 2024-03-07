using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

[ExecuteInEditMode]
public class SensorSupport : MonoBehaviour
{
    protected void OnEnable()
    {
        // GameDataManager.sensorAdded -= OnSensorAdded;
        // GameDataManager.sensorRemoved -= OnSensorRemoved;
        // GameDataManager.sensorAdded += OnSensorAdded;
        // GameDataManager.sensorRemoved += OnSensorRemoved;
    }

    protected void OnDisable()
    {
        // GameDataManager.sensorAdded -= OnSensorAdded;
        // GameDataManager.sensorRemoved -= OnSensorRemoved;
    }

    private void OnSensorAdded(string name, string address)
    {
        SyncsenseSensor sensor = (SyncsenseSensor) InputSystem.AddDevice(
            new InputDeviceDescription
            {
                interfaceName = "GameDataManager",
            });
        sensor.controllerName = name;
        sensor.deviceId = address;
    }

    private void OnSensorRemoved(string name)
    {
        SyncsenseSensor device = (SyncsenseSensor) InputSystem.devices.FirstOrDefault(
            x => x.description == new InputDeviceDescription
            {
                interfaceName = "GameDataManager",
            });

        if (device != null)
            InputSystem.RemoveDevice(device);
    }

    // Move the registration of MyDevice from the
    // static constructor to here, and change the
    // registration to also supply a matcher.
    protected void Awake()
    {
        // Add a match that catches any Input Device that reports its
        // interface as "ThirdPartyAPI".
        InputSystem.RegisterLayout<SyncsenseSensor>(
            matches: new InputDeviceMatcher()
                .WithInterface("GameDataManager"));
    }
}
