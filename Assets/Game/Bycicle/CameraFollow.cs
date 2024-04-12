using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace Game.Bicycle
{
    public class CameraFollow : MonoBehaviour
    {
        public GameManager gameManager;
        public Transform player;
        public Vector3 positionOffset;
        public Vector3 lookAtOffset;

        // Update is called once per frame
        void FixedUpdate()
        {
            Vector3 targetPosition = player.position + positionOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, 3 * Time.deltaTime);

            //Vector3 direction = player.position + lookAtOffset - transform.position;
            //Quaternion toRotation = Quaternion.FromToRotation(transform.rotation.eulerAngles, direction);
            //transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 2 * Time.deltaTime);

            Vector3 targetLookAt = Vector3.Lerp(transform.position + transform.forward, player.position + lookAtOffset, Time.deltaTime);
            transform.LookAt(targetLookAt);
        }
    }
}


