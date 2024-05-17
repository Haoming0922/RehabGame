using System;
using System.Collections;
using System.Collections.Generic;
using Game.Avatar;
using TMPro;
using UnityEngine;
using Game.Sensor;
using Game.Util;

namespace Game.JumpJump
{

    public class Player : MonoBehaviour
    {
        public SensorManager sensorManager;
        public GameManager gameManager;
        public JumpUIUpdater UIUpdater;
        
        public Rigidbody player;
        public GameObject curve;
        
        private GameObject mass;
        
        private float force = 0;
        private Vector3 jumpPosition = new Vector3();

        private bool isOnGround = true;

        private JumpAnimation jumpAnimation;

        private int currentCube;

        private float jumpTimeStep;

        public CameraFollow cameraFollow;
        
        void Start()
        {
            // Application.targetFrameRate = -1;
            mass = transform.GetChild(0).gameObject;
            // SetMass();
            jumpAnimation = GetComponent<JumpAnimation>();
            jumpAnimation.Idle();
            jumpTimeStep = Time.time;
        }
        

        private void Update()
        {
            if (gameManager.state == GameState.PLAY)
            {
                SensorJump(); // TODO
                // keyBoardInput();
                UIUpdater.UpdateForceUI(force);
            }
        }

        private void SensorJump()
        {
            if (isOnGround)
            {
                force = sensorManager.GetData(SensorPosition.LEFT) + sensorManager.GetData(SensorPosition.RIGHT);
                UIUpdater.DisplayOneSide();
                if (IsJump())
                {
                    // DrawCurve(jumpPosition);
                    UIUpdater.DisplayJumpUI(false);
                    gameManager.UpdatePlayerData(force);
                    StartCoroutine(MoveAlongCurve(jumpPosition));
                    force = 0;
                    // jumpTimeStep = Time.time;
                }
            }
            else
            {
                UIUpdater.DisplayJumpUI(false);
                sensorManager.ResetData();
                force = 0;
            }
            
            // //
            // if (input > 0)
            // {
            //     force = input;
            // }

            // if (Time.time - jumpTimeStep > 3f && isOnGround)
            // {
            //     UIUpdater.DisplayOneSide();
            //     Debug.Log("JumpJump Intervals: " + (Time.time - jumpTimeStep));
            //     if (IsJump())
            //     {
            //         Debug.Log("JumpJump Force: " + force);
            //         // DrawCurve(jumpPosition);
            //         UIUpdater.DisplayJumpUI(false);
            //         gameManager.UpdatePlayerData(force);
            //         StartCoroutine(MoveAlongCurve(jumpPosition));
            //         force = 0;
            //         jumpTimeStep = Time.time;
            //     }
            // }
            // else
            // {
            //     UIUpdater.DisplayJumpUI(false);
            //     force = 0;
            // }
            //

        }
        

        private void keyBoardInput()
        {
            if (Input.GetKey(KeyCode.J) && isOnGround)
            {
                force += 0.1f;
                // if (force >= 0.2 && (jumpPosition - transform.position).magnitude > 2f)
                // {
                //     DrawCurve(jumpPosition);
                // }
            }

            if (!Input.GetKey(KeyCode.J) && IsJump() && isOnGround)
            {
                UIUpdater.DisplayJumpUI(false);
                gameManager.UpdatePlayerData(force);
                StartCoroutine(MoveAlongCurve(jumpPosition));
                force = 0;
            }

            // Debug.Log(force);
        }


        private IEnumerator MoveAlongCurve(Vector3 jumpPosition)
        {
            
            // for (int i = 0; i < curve.transform.childCount; i++)
            // {
            //     curve.transform.GetChild(i).gameObject.SetActive(false);
            // }

            // Bezier Curve
            float height = Mathf.Max(transform.position.y, jumpPosition.y);
            Vector3 p0 = transform.position;
            Vector3 p1 = new Vector3(transform.position.x, height+ 5f, transform.position.z + 1f);
            Vector3 p2 = new Vector3(jumpPosition.x, height + 6f, jumpPosition.z + 4f);
            Vector3 p3 = jumpPosition;

            // this.GetComponent<Rigidbody>().useGravity = false;

            cameraFollow.enabled = false;
            
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
                // if (currentCube < gameManager.totalCubes - 1)
                // {
                //     transform.LookAt(GetLookAt(transform.position));
                // }
                yield return null;
            }

            // t = 0;
            // while (t < 1)
            // {
            //     Vector3 targetLookAt = Vector3.Lerp(transform.position + transform.forward,
            //         gameManager.GetTargetPosition(), Time.deltaTime);
            //     transform.LookAt(targetLookAt);
            //     t += 1f * Time.deltaTime;
            // }

            // if (currentCube < gameManager.totalCubes - 1)
            // {
            //     gameManager.SetControlSensor(null);
            // }
            Debug.Log("A" + currentCube);

            cameraFollow.enabled = true;

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
            Vector3 p1 = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z);
            Vector3 p2 = new Vector3(jumpPosition.x, jumpPosition.y + 10, jumpPosition.z + 1);
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
            
            bool isJump = force > 1000f;

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
                isOnGround = true;
                currentCube = other.gameObject.transform.parent.GetSiblingIndex();
                gameManager.UpdateCurrentCube(currentCube);
                gameManager.DisableCurrentLastCube(currentCube-1);
                // jumpAnimation.Idle();
                //cam.LookAt(transform.position);
            }
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (other.transform.parent.gameObject.CompareTag("Finish"))
            {
                // Debug.Log("Finish");
                gameManager.gameEndEvent?.Invoke();
                gameObject.GetComponent<Player>().enabled = false;
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

        public Vector3 GetLookAt(Vector3 currentPosition_)
        {
            Vector3 dir = gameManager.GetTargetPosition();
            dir.y = currentPosition_.y;
            dir = Vector3.Lerp(transform.position + transform.forward, dir, Time.deltaTime);
            return dir;
        }
    }

}