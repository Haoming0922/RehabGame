using System;

public class SensorDataReceived: RawDataReceived
{
    public int index;
    public float accX;
    public float accY;
    public float accZ;
    public float gyroX;
    public float gyroY;
    public float gyroZ;

    public SensorDataReceived()
    {
        deviceAddress = "";
        index = 0;
        accX = 0;
        accY = 0;
        accZ = 0;
        gyroX = 0;
        gyroY = 0;
        gyroZ = 0;
    }
    
    public SensorDataReceived Init()
    {
        index = data[0];
        accX = BitConverter.ToSingle(data, 1);
        accY = BitConverter.ToSingle(data, 5);
        accZ = BitConverter.ToSingle(data, 9);
        gyroX = BitConverter.ToSingle(data, 13);
        gyroY = BitConverter.ToSingle(data, 17);
        gyroZ = BitConverter.ToSingle(data, 21);
        
        return this;
    }

    public string ToStringCSV()
    {
        return deviceAddress + "," + DateTime.Now.ToString("HH:mm:ss.fff") + "," + index + "," + accX + "," + accY +
               "," + accZ + "," + gyroX + "," + gyroY + "," + gyroZ;
    }
    
    public static SensorDataReceived operator +(SensorDataReceived data1, SensorDataReceived data2)
    {
        if (data1 == null && data2 == null)
        {
            return new SensorDataReceived();
        }
        if (data1 == null) return data2;
        if (data2 == null) return data1;

        SensorDataReceived result = new SensorDataReceived();

        result.deviceAddress = data2.deviceAddress;
        result.index = data2.index;
        result.accX = data1.accX + data2.accX;
        result.accY = data1.accY + data2.accY;
        result.accZ = data1.accZ + data2.accZ;
        result.gyroX = data1.gyroX + data2.gyroX;
        result.gyroY = data1.gyroY + data2.gyroY;
        result.gyroZ = data1.gyroZ + data2.gyroZ;

        return result;
    }
    
    public static SensorDataReceived operator /(SensorDataReceived data, int divisor)
    {
        if (data == null)
        {
            return new SensorDataReceived();
        }

        if (divisor == 0)
        {
            throw new ArgumentException("Divisor cannot be zero.");
        }

        SensorDataReceived result = new SensorDataReceived();
        result.deviceAddress = data.deviceAddress;
        result.index = data.index;
        result.accX = data.accX / divisor;
        result.accY = data.accY / divisor;
        result.accZ = data.accZ / divisor;
        result.gyroX = data.gyroX / divisor;
        result.gyroY = data.gyroY / divisor;
        result.gyroZ = data.gyroZ / divisor;

        return result;
    }
    
}