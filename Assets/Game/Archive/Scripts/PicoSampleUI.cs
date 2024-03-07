using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class PicoSampleUI : MonoBehaviour
{
    public Text left;
    private InputDevice leftDevice;

    private void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, devices);
        if(devices.Count > 0) leftDevice = devices[0];

        StartCoroutine(ReadDataCoroutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ReadDataCoroutine()
    {
        while (true)
        {
            if (leftDevice.isValid)
            {
                if (leftDevice.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out Vector3 leftAngularVelocity) && 
                    leftDevice.TryGetFeatureValue(CommonUsages.deviceAcceleration, out Vector3 leftAcceleration) && 
                    leftDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 leftVelocity) && 
                    leftDevice.TryGetFeatureValue(CommonUsages.deviceAngularAcceleration, out Vector3 leftAngularAcceleration))
                {
                    left.text = 
                        "left Acceleration: " + leftAcceleration + '\n' +
                        "left Velocity: " + leftVelocity + '\n' +
                        "left AngularVelocity: " + leftAngularVelocity + '\n' +
                        "left AngularAcceleration: " + leftAngularAcceleration + '\n' ;
                
                    yield return new WaitForSeconds(0.3f);
                }
            }
        }
    }

    void ReadData()
    {
        if (leftDevice.isValid)
        {
            
            if (leftDevice.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out Vector3 leftAngularVelocity) && 
                leftDevice.TryGetFeatureValue(CommonUsages.deviceAcceleration, out Vector3 leftAcceleration) && 
                leftDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 leftVelocity) && 
                leftDevice.TryGetFeatureValue(CommonUsages.deviceAngularAcceleration, out Vector3 leftAngularAcceleration))
            {
                left.text = 
                    "left Acceleration: " + leftAcceleration + '\n' +
                    "left Velocity: " + leftVelocity + '\n' +
                    "left AngularVelocity: " + leftAngularVelocity + '\n' +
                    "left AngularAcceleration: " + leftAngularAcceleration + '\n' ;
            }
        }
    }
}
