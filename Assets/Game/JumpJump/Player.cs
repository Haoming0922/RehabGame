using System;
using System.Collections;
using System.Collections.Generic;
using Game.Avatar;
using TMPro;
using UnityEngine;
using Game.Sensor;

namespace Game.JumpJump
{

    public class Player : MonoBehaviour
    {
        public SensorManager sensorManager;
        public GameManager gameManager;
        public Rigidbody player;
        public GameObject curve;
        
        private GameObject mass;
        
        private float force = 0;
        private Vector3 jumpPosition = new Vector3();

        private bool isOnGround = true;

        private JumpAnimation jumpAnimation;
        
        void Start()
        {
            // Application.targetFrameRate = -1;
            mass = transform.GetChild(0).gameObject;
            // SetMass();
            jumpAnimation = GetComponent<JumpAnimation>();
            jumpAnimation.Idle();
        }
        

        private void Update()
        {
            SensorJump();
            // keyBoardInput();
        }

        private void SensorJump()
        {
            float input = sensorManager.GetData(gameManager.currentController);

            if (input > 0)
            {
                force = input;
            }
            
            if (input < 0 && IsJump() && isOnGround)
            {
                gameManager.ToggleText(false);
                StartCoroutine(MoveAlongCurve(jumpPosition));
                force = 0;
            }
        }

        private void keyBoardInput()
        {
            if (Input.GetKey(KeyCode.J) && isOnGround)
            {
                force += 0.01f;
                // if (force >= 0.2 && (jumpPosition - transform.position).magnitude > 2f)
                // {
                //     DrawCurve(jumpPosition);
                // }
            }

            if (!Input.GetKey(KeyCode.J) && IsJump() && isOnGround)
            {
                gameManager.ToggleText(false);
                StartCoroutine(MoveAlongCurve(jumpPosition));
                force = 0;
            }

            // Debug.Log(force);
        }


        private IEnumerator MoveAlongCurve(Vector3 jumpPosition)
        {
            for (int i = 0; i < curve.transform.childCount; i++)
            {
                curve.transform.GetChild(i).gameObject.SetActive(false);
            }

            // Bezier Curve
            Vector3 p0 = transform.position;
            Vector3 p1 = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z);
            Vector3 p2 = new Vector3(jumpPosition.x, jumpPosition.y + 6, jumpPosition.z);
            Vector3 p3 = jumpPosition;

            // this.GetComponent<Rigidbody>().useGravity = false;
            
            float t = 0;
            while (t < 1)
            {
                Vector3 bt = Mathf.Pow(1 - t, 3) * p0 +
                             3 * t * Mathf.Pow(1 - t, 2) * p1 +
                             3 * (1 - t) * Mathf.Pow(t, 2) * p2 +
                             Mathf.Pow(t, 3) * p3;
                
                // int lerp = 0;
                // while (lerp < 30)
                // {
                //     transform.position = Vector3.Lerp(transform.position, bt, Time.deltaTime);
                //     lerp++;
                // }
                transform.position = bt;
                //transform.Rotate(rotationAngle * Time.deltaTime);
                t += 1f * Time.deltaTime;
                yield return null;
            }

            // this.GetComponent<Rigidbody>().useGravity = true;
        }

        private void SetMass()
        {
            player.centerOfMass = Vector3.down;
            mass.transform.position = transform.position + player.centerOfMass;
        }


        private void DrawCurve(Vector3 jumpPosition)
        {
            // Bezier Curve
            Vector3 p0 = transform.position;
            Vector3 p1 = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z + 3);
            Vector3 p2 = new Vector3(jumpPosition.x, jumpPosition.y + 6, jumpPosition.z);
            Vector3 p3 = jumpPosition;
            
            for (int i = 0; i < curve.transform.childCount; i++)
            {
                float t = 0.15f * i;
                Vector3 bt = Mathf.Pow(1 - t, 3) * p0 +
                             3 * t * Mathf.Pow(1 - t, 2) * p1 +
                             3 * (1 - t) * Mathf.Pow(t, 2) * p2 +
                             Mathf.Pow(t, 3) * p3;

                curve.transform.GetChild(i).gameObject.SetActive(true);
                curve.transform.GetChild(i).position = bt;

                //if (lastSphere == null || EnoughSpace(lastSphere.position, bt))
                //{
                //    curve.transform.GetChild(i).gameObject.SetActive(true);
                //    curve.transform.GetChild(i).position = bt;
                //    lastSphere = curve.transform.GetChild(i);
                //}
                //else
                //{
                //    return i;
                //}
            }
        }
        

        private bool IsJump()
        {
            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = gameManager.GetTargetPosition();
            // float targetForce =  (targetPosition - currentPosition).magnitude / gameManager.MaxDistance;
            // targetForce = Mathf.Clamp(targetForce, 0f, 1f);
            // return currentPosition + force / targetForce * (targetPosition - currentPosition);
            
            bool isJump = force > 0.5f ? true : false;

            if (isJump)
            {
                jumpPosition = targetPosition;
                return true;
            }
            else return false;
        }

        private void OnCollisionStay(Collision other)
        {
            if (other.gameObject.CompareTag("Ground"))
            {
                gameManager.UpdateCurrentCube(other.gameObject.transform.parent.GetSiblingIndex());
                isOnGround = true;
                // jumpAnimation.Idle();
                //cam.LookAt(transform.position);
            }
        }
        
        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("Ground"))
            {
                isOnGround = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Dead"))
            {
                transform.rotation = Quaternion.identity;
                transform.position = gameManager.GetCurrentPosition();
;            }
        }

        public Vector3 GetLookAt()
        {
            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = gameManager.GetTargetPosition();
            return (currentPosition + targetPosition) / 2;
        }
    }

}