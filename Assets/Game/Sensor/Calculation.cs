using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

namespace Game.Sensor
{
    public class Calculation
    {
        public static bool IsMove(SensorDataReceived data)
        {
            float moving = Mathf.Abs(data.accX) + Mathf.Abs(data.accY) + Mathf.Abs(data.accZ);
            // Debug.Log("[Haoming] IsMove: " + moving);
            return moving > 15f;
        }
        
        public static float IsCycle(SensorDataReceived data)
        {
            float moving = Mathf.Abs(data.accX) + Mathf.Abs(data.accY) + Mathf.Abs(data.accZ);
            // Debug.Log("[Haoming] IsMove: " + moving);
            return Mathf.Clamp(moving / 50f, 0 ,1f);
        }
        
        public static bool IsRaise(SensorDataReceived data)
        {
            float threshold = 10;
            float sign = data.accX >= 0 ? 1 : -1;
            return data.gyroY * sign > threshold;
        }
        
        public static bool IsDownX(SensorDataReceived data)
        {
            // Debug.Log("[Haoming] IsDownX: " + Mathf.Abs(data.accX));
            return Mathf.Abs(data.accX) > 8f;
        }


        public static float RotationAmount(SensorDataReceived data)
        {
            return Mathf.Abs(data.gyroX) + Mathf.Abs(data.gyroY) + Mathf.Abs(data.gyroZ);
        }

        public static float ToRacingData(SensorDataReceived data)
        {
            float threshold = 12f;
            float averageMotion = Mathf.Abs(data.accX) + Mathf.Abs(data.accY) + Mathf.Abs(data.accZ);
            if (averageMotion > threshold) return Mathf.Log10(averageMotion / threshold);
            else return 0;
        }
        
        public static float GetRestGravity(SensorDataReceived data)
        {
            float averageMotion = Mathf.Pow(data.accX, 2) + Mathf.Pow(data.accY, 2) + Mathf.Pow(data.accZ, 2);
            return Mathf.Sqrt(averageMotion);
        }

        public static SensorDataReceived AverageQueue(Queue<SensorDataReceived> dataQueue)
        {
            SensorDataReceived sum = new SensorDataReceived();
            foreach (var data in dataQueue)
            {
                sum += data;
            }

            return sum / dataQueue.Count;
        }
        
        public static float AverageQueue(Queue<float> dataQueue)
        {
            float sum = 0;
            foreach (var data in dataQueue)
            {
                sum += data;
            }

            return sum / dataQueue.Count;
        }
        
        
        public static float ComplementaryFilterRotationY(float gy, float ax, float angleY, float weightAcc, float gravity)
        {
            // Clamp ax value within range
            if (ax < -Mathf.Abs(gravity)) ax = -Mathf.Abs(gravity);
            else if (ax > Mathf.Abs(gravity)) ax = Mathf.Abs(gravity);

            // Calculate thetaAcc (angle from accelerometer)
            float thetaAcc = Mathf.Acos(ax / gravity) * Mathf.Rad2Deg;

            // Calculate thetaRot (angle from gyroscope)
            float sign = ax >= 0 ? 1 : -1;
            float thetaRot = angleY + gy * sign * Time.deltaTime;

            // Debug.Log("[Haoming] thetaAcc: " + thetaAcc + ", thetaRot: " + thetaRot);
            
            // Calculate the final angle using the complementary filter
            return weightAcc * thetaAcc + (1 - weightAcc) * thetaRot;
        }



        public static Quaternion MadgwickIMU(float gx, float gy, float gz, float ax, float ay, float az,
            float deltaTime, Quaternion q)
        {
            float Beta = 0.1f; // algorithm gain

            float q1 = q[0], q2 = q[1], q3 = q[2], q4 = q[3]; // short name local variable for readability
            float norm;
            float s1, s2, s3, s4;
            float qDot1, qDot2, qDot3, qDot4;

            // Auxiliary variables to avoid repeated arithmetic
            float _2q1 = 2f * q1;
            float _2q2 = 2f * q2;
            float _2q3 = 2f * q3;
            float _2q4 = 2f * q4;
            float _4q1 = 4f * q1;
            float _4q2 = 4f * q2;
            float _4q3 = 4f * q3;
            float _8q2 = 8f * q2;
            float _8q3 = 8f * q3;
            float q1q1 = q1 * q1;
            float q2q2 = q2 * q2;
            float q3q3 = q3 * q3;
            float q4q4 = q4 * q4;

            // Normalise accelerometer measurement
            norm = (float)Mathf.Sqrt(ax * ax + ay * ay + az * az);
            if (norm == 0f) norm = 0.001f; // handle NaN
            norm = 1 / norm; // use reciprocal for division
            ax *= norm;
            ay *= norm;
            az *= norm;

            // Gradient decent algorithm corrective step
            s1 = _4q1 * q3q3 + _2q3 * ax + _4q1 * q2q2 - _2q2 * ay;
            s2 = _4q2 * q4q4 - _2q4 * ax + 4f * q1q1 * q2 - _2q1 * ay - _4q2 + _8q2 * q2q2 + _8q2 * q3q3 + _4q2 * az;
            s3 = 4f * q1q1 * q3 + _2q1 * ax + _4q3 * q4q4 - _2q4 * ay - _4q3 + _8q3 * q2q2 + _8q3 * q3q3 + _4q3 * az;
            s4 = 4f * q2q2 * q4 - _2q2 * ax + 4f * q3q3 * q4 - _2q3 * ay;
            norm = 1f / (float)Mathf.Sqrt(s1 * s1 + s2 * s2 + s3 * s3 + s4 * s4); // normalise step magnitude
            s1 *= norm;
            s2 *= norm;
            s3 *= norm;
            s4 *= norm;

            // Compute rate of change of quaternion
            qDot1 = 0.5f * (-q2 * gx - q3 * gy - q4 * gz) - Beta * s1;
            qDot2 = 0.5f * (q1 * gx + q3 * gz - q4 * gy) - Beta * s2;
            qDot3 = 0.5f * (q1 * gy - q2 * gz + q4 * gx) - Beta * s3;
            qDot4 = 0.5f * (q1 * gz + q2 * gy - q3 * gx) - Beta * s4;

            // Integrate to yield quaternion
            q1 += qDot1 * deltaTime;
            q2 += qDot2 * deltaTime;
            q3 += qDot3 * deltaTime;
            q4 += qDot4 * deltaTime;
            norm = 1f / (float)Mathf.Sqrt(q1 * q1 + q2 * q2 + q3 * q3 + q4 * q4); // normalise quaternion
            q[0] = q1 * norm;
            q[1] = q2 * norm;
            q[2] = q3 * norm;
            q[3] = q4 * norm;
            return q;
        }

        
        
        
    }

}