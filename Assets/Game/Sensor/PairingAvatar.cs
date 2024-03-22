using System;
using System.Collections;
using System.Collections.Generic;
using Game.Avatar;
using Game.Sensor;
using UnityEngine;
using Game.Util;
using Unity.Burst.Intrinsics;

public class PairingAvatar : MonoBehaviour
{
    [SerializeField] private Vector3 leftStartRotation;
    [SerializeField] private Vector3 rightStartRotation;
    [SerializeField] private ThirdPersonLoader loader;

    private Transform leftShoulder;
    private Transform rightShoulder;
    private Transform leftArm;
    private Transform rightArm;
    private Transform leftForeArm;
    private Transform rightForeArm;

    public void FindArm()
    {
        leftShoulder = GameObject.Find("LeftShoulder").transform;
        rightShoulder = GameObject.Find("RightShoulder").transform;
        leftArm = GameObject.Find("LeftArm").transform;
        rightArm = GameObject.Find("RightArm").transform;
        leftForeArm = GameObject.Find("LeftForeArm").transform;
        rightForeArm = GameObject.Find("RightForeArm").transform;
    }
    
    public IEnumerator PutArmDown()
    {
        float elapsedTime = 0;
        float speed = 1f;
        
        while (elapsedTime < 3f)
        {
            leftShoulder.localRotation = Quaternion.Lerp(leftShoulder.localRotation, Quaternion.Euler(100f, leftShoulder.localRotation.eulerAngles.y, leftShoulder.localRotation.eulerAngles.z), speed * Time.deltaTime);
            rightShoulder.localRotation = Quaternion.Lerp(rightShoulder.localRotation, Quaternion.Euler(95f, rightShoulder.localRotation.eulerAngles.y, rightShoulder.localRotation.eulerAngles.z), speed * Time.deltaTime);
            
            leftArm.localRotation = Quaternion.Lerp(leftArm.localRotation, Quaternion.Euler(leftStartRotation), speed * Time.deltaTime);
            rightArm.localRotation = Quaternion.Lerp(rightArm.localRotation, Quaternion.Euler(rightStartRotation), speed * Time.deltaTime);
            
            leftForeArm.localRotation = Quaternion.Lerp(leftForeArm.localRotation, Quaternion.Euler(0, 0, -5), speed * Time.deltaTime);
            rightForeArm.localRotation = Quaternion.Lerp(rightForeArm.localRotation, Quaternion.Euler(0, 0, -5), speed * Time.deltaTime);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    

    public void UpdateArmRotation(SensorPosition position, float angle)
    {
        if (angle < 0) return;
        // mirror user movement
        switch (position)
        {
            case SensorPosition.LEFT:
                rightArm.localRotation = Quaternion.Euler(rightStartRotation.x - angle, rightStartRotation.y, rightStartRotation.z);
                break;
            case SensorPosition.RIGHT:
                leftArm.localRotation = Quaternion.Euler(leftStartRotation.x - angle, leftStartRotation.y, leftStartRotation.z);
                break;
        }
    }
}
