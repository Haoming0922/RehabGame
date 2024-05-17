using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Game.Util;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.JumpJump
{
    public class CameraFollow : MonoBehaviour
    {
        public GameManager gameManager;
        public Transform player;
        public Camera playerHead;
        
        // Update is called once per frame
        void FixedUpdate()
        {
            Follow();
            // if (Reload.action.IsPressed()) ResetPosition();
            // else
            // {
            //     Follow();
            // }
        }

        void Follow()
        {
            Vector3 targetPosition = player.position - 5.5f * player.forward + 3f * player.up;
            transform.position = Vector3.Lerp(transform.position, targetPosition, 1.3f * Time.deltaTime);
            
            // transform.rotation = Quaternion.Lerp(transform.rotation,  player.rotation, 2 * Time.deltaTime);
            
            Vector3 targetLookAt = Vector3.Lerp(transform.position + transform.forward, new Vector3(gameManager.GetTargetPosition().x, transform.position.y , gameManager.GetTargetPosition().z),  .1f * Time.deltaTime);
            // Vector3 targetLookAt = Vector3.Lerp(transform.position + transform.forward, player.position + lookAtOffset,  Time.deltaTime);
            transform.LookAt(targetLookAt);
        }
        
        void ResetPosition()
        {
            float rotationY = player.rotation.eulerAngles.y - playerHead.transform.rotation.eulerAngles.y;
            transform.Rotate(0, rotationY, 0);
            
            Vector3 distanceDiff = player.position - playerHead.transform.position;
            transform.position += distanceDiff;
        }
        
    }
}


