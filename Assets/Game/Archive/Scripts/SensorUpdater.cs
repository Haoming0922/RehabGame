using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class SensorUpdater
{
    public string deviceAddress;
    public SensorDataReceived data;
    public SensorDataReceived idleData;
    public int sampleRate;
    
    public Stopwatch stopwatch;
    public long lastUpdateTime;
    public int samplesCounter;

    public SensorUpdater()
    {
        stopwatch = new Stopwatch();
        lastUpdateTime = 0;
        samplesCounter = 0;
        sampleRate = 0;
        data = null;
    }
    
}
