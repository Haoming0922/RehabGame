
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class DeviceUIUpdater
{
    private TextMeshProUGUI _textSamplesPerSecondDevice;
    private TextMeshProUGUI _textAccXDevice;
    private TextMeshProUGUI _textAccYDevice;
    private TextMeshProUGUI _textAccZDevice;
    private TextMeshProUGUI _textGyroXDevice;
    private TextMeshProUGUI _textGyroYDevice;
    private TextMeshProUGUI _textGyroZDevice;
    
    private TextMeshProUGUI _textBatteryDevice;

    private volatile int _lastIndex = -1;
    
    private int _samplesCounter = 0;
    private Stopwatch _stopwatch = new Stopwatch();
    private long _lastUpdateTime = 0;
    
    public DeviceUIUpdater(TextMeshProUGUI textSamplesPerSecondDevice, TextMeshProUGUI textAccXDevice, TextMeshProUGUI textAccYDevice, TextMeshProUGUI textAccZDevice, TextMeshProUGUI textGyroXDevice, TextMeshProUGUI textGyroYDevice, TextMeshProUGUI textGyroZDevice, TextMeshProUGUI textBatteryDevice)
    {
        _textSamplesPerSecondDevice = textSamplesPerSecondDevice;
        _textAccXDevice = textAccXDevice;
        _textAccYDevice = textAccYDevice;
        _textAccZDevice = textAccZDevice;
        _textGyroXDevice = textGyroXDevice;
        _textGyroYDevice = textGyroYDevice;
        _textGyroZDevice = textGyroZDevice;
        _textBatteryDevice = textBatteryDevice;
        _stopwatch.Start();
    }

    public void HandleSensorData(SensorDataReceived data)
    {   
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            _textAccXDevice.SetText("Acc X: " + data.accX);
            _textAccYDevice.SetText("Acc Y: " + data.accY);
            _textAccZDevice.SetText("Acc Z: " + data.accZ);
            _textGyroXDevice.SetText("Gyro X: " + data.gyroX);
            _textGyroYDevice.SetText("Gyro Y: " + data.gyroY);
            _textGyroZDevice.SetText("Gyro Z: " + data.gyroZ);
        });
        
        _samplesCounter++;

        // Check if one second has passed
        if (_stopwatch.ElapsedMilliseconds - _lastUpdateTime >= 1000)
        {
            int tempSamplesCounter = _samplesCounter;
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                _textSamplesPerSecondDevice.SetText("Samples per second: " + tempSamplesCounter);
            });
            
            // Reset counter
            _samplesCounter = 0;

            // Update the last update time
            _lastUpdateTime = _stopwatch.ElapsedMilliseconds;
        }
    }

    public void HandleBatteryData(BatteryDataReceived data)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            _textBatteryDevice.SetText("Battery Level: " + data.batteryLevel + " % ");
        });
    }

}