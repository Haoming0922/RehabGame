
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class DeviceTransformUpdater
{

    private GameObject _cube;
    private Vector3 _currentRotation = Vector3.zero;
    private bool _sensorFusion = false;
    // private MadgwickIMU _madgwick = new MadgwickIMU();
    
    public DeviceTransformUpdater(GameObject cube, bool sensorFusion)
    {
        _cube = cube;
        _sensorFusion = sensorFusion;
    }

    public void HandleData(SensorDataReceived data)
    {
        if (_sensorFusion)
            ApplyTransformWithSensorFusion(data);
        else 
            ApplyTransformWithoutSensorFusion(data);
    }
    
    

    // Assuming this is called whenever new data is received
    public void ApplyTransformWithoutSensorFusion(SensorDataReceived data)
    {
        // Use the time elapsed between updates for integration.
        // NOTE: This assumes that you're calling this method frequently.
        // If you have a fixed interval of receiving data, replace Time.deltaTime with that interval.
        float deltaTime = Time.deltaTime;

        // Integrate angular velocity to get the rotation delta
        _currentRotation.x += data.gyroX * deltaTime;
        _currentRotation.y += data.gyroY * deltaTime;
        _currentRotation.z += data.gyroZ * deltaTime;

        // Apply rotation to the cube
        _cube.transform.eulerAngles = _currentRotation;
    }
    
    public void ApplyTransformWithSensorFusion(SensorDataReceived data)
    {
        // Convert degrees/second to radians/second
        float gx = Mathf.Deg2Rad * data.gyroX;
        float gy = Mathf.Deg2Rad * data.gyroY;
        float gz = Mathf.Deg2Rad * data.gyroZ;

        float deltaTime = Time.deltaTime;

        // _madgwick.Update(gx, gy, gz, data.accX, data.accY, data.accZ, deltaTime);
        //
        // // Apply the orientation to the cube
        // _cube.transform.rotation = _madgwick.Quaternion;
    }

}