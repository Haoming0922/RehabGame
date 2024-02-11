using UnityEngine;

public class MadgwickAHRS
{
    public float Beta = 0.1f; // algorithm gain
    public Quaternion Quaternion = new Quaternion(1, 0, 0, 0); // output quaternion describing the Earth relative to the sensor

    public void Update(float gx, float gy, float gz, float ax, float ay, float az, float deltaTime)
    {
        float q0 = Quaternion.w, q1 = Quaternion.x, q2 = Quaternion.y, q3 = Quaternion.z;
        float norm;
        float f1, f2, f3;
        float s1, s2, s3;
        float qDot1, qDot2, qDot3, qDot4;

        // Auxiliary variables to avoid repeated arithmetic
        float _2q0 = 2f * q0;
        float _2q1 = 2f * q1;
        float _2q2 = 2f * q2;
        float _2q3 = 2f * q3;
        float _4q0 = 4f * q0;
        float _4q1 = 4f * q1;
        float _4q2 = 4f * q2;
        float _4q3 = 4f * q3;
        float _8q1 = 8f * q1;
        float _8q2 = 8f * q2;
        float _8q3 = 8f * q3;
        float q0q0 = q0 * q0;
        float q1q1 = q1 * q1;
        float q2q2 = q2 * q2;
        float q3q3 = q3 * q3;

        // Normalize accelerometer measurement
        norm = Mathf.Sqrt(ax * ax + ay * ay + az * az);
        if (norm == 0f) return; // handle NaN
        norm = 1 / norm;        // use reciprocal for division
        ax *= norm;
        ay *= norm;
        az *= norm;

        // Gradient descent algorithm corrective step
        f1 = _2q2 * q3 - _2q1 * q0 - ax;
        f2 = _2q0 * q1 + _2q3 * q2 - ay;
        f3 = 1f - _2q1 * q1 - _2q2 * q2 - az;
        s1 = _4q0 * q2 + _2q3 * ax + _4q1 * q0 - _2q1 * ay - _8q2 * q1 + _8q2 * q2 - _4q3 * q3 + _2q3 * az;
        s2 = _4q0 * q3 - _2q2 * ax + _4q2 * q0 - _2q2 * ay - _8q2 * q1 + _8q2 * q2 + _4q1 * q3 - _2q1 * az;
        s3 = _4q1 * q1 + _2q1 * ax + _4q2 * q2 + _2q2 * ay + _4q3 * q0 - _2q3 * az - _8q3 * q3 + _8q3 * q3;

        // Normalize step magnitude
        norm = Mathf.Sqrt(s1 * s1 + s2 * s2 + s3 * s3);
        norm = 1f / norm;
        s1 *= norm;
        s2 *= norm;
        s3 *= norm;

        // Apply feedback step
        gx = gx - Beta * s1;
        gy = gy - Beta * s2;
        gz = gz - Beta * s3;

        // Integrate rate of change of quaternion
        qDot1 = 0.5f * (-q1 * gx - q2 * gy - q3 * gz);
        qDot2 = 0.5f * (q0 * gx + q2 * gz - q3 * gy);
        qDot3 = 0.5f * (q0 * gy - q1 * gz + q3 * gx);
        qDot4 = 0.5f * (q0 * gz + q1 * gy - q2 * gx);

        // Update estimation
        q0 += qDot1 * deltaTime;
        q1 += qDot2 * deltaTime;
        q2 += qDot3 * deltaTime;
        q3 += qDot4 * deltaTime;

        // Normalize quaternion
        norm = Mathf.Sqrt(q0 * q0 + q1 * q1 + q2 * q2 + q3 * q3);
        norm = 1f / norm;
        Quaternion.w = q0 * norm;
        Quaternion.x = q1 * norm;
        Quaternion.y = q2 * norm;
        Quaternion.z = q3 * norm;
    }
}
