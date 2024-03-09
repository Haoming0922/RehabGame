using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Game.Core;

namespace Game.JumpJump
{

    public class Player : MonoBehaviour
    {
        public SensorManager sensorManager;
        public GameManager gameManager;
        public Rigidbody player;
        public GameObject curve;
        
        [Header("Jump Force Parameters")]
        public float maxForceThreshold;
        public float forceFactor;
        
        private GameObject mass;
        
        private float force = 0;
        private float targetForce = 0;

        private bool isOnGround = false;
        
        void Start()
        {
            // Application.targetFrameRate = -1;
            mass = transform.GetChild(0).gameObject;
            SetMass();
        }

        void FixedUpdate()
        {
            Jump();
        }

        private void Jump()
        {
            Vector3 jumpPosition = GetEndPosition();

            SensorInput(jumpPosition);
            // keyBoardInput(jumpPosition);
        }

        private void SensorInput(Vector3 position)
        {
            float sensorInput = sensorManager.GetData(gameManager.currentController);
            if (sensorInput > 0 && isOnGround)
            {
                force += sensorInput;
                if (position != transform.position) DrawCurve(position);
            }
            else
            {
                if (force > 0)
                {
                    StartCoroutine(MoveAlongCurve(position));
                    force = 0;
                }
            }
        }

        private void keyBoardInput(Vector3 position)
        {
            if (Input.GetKey(KeyCode.J) && isOnGround)
            {
                force++;
                if (position != transform.position) DrawCurve(position);
                else
                {
                    for (int i = 0; i < curve.transform.childCount; i++)
                    {
                        curve.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                if (force > 0)
                {
                    StartCoroutine(MoveAlongCurve(position));
                    force = 0;
                }
            }
        }


        private IEnumerator MoveAlongCurve(Vector3 jumpPosition)
        {
            for (int i = 0; i < curve.transform.childCount; i++)
            {
                curve.transform.GetChild(i).gameObject.SetActive(false);
            }

            // Bezier Curve
            Vector3 p0 = transform.position;
            Vector3 p1 = new Vector3(transform.position.x, jumpPosition.y + 6, transform.position.z);
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
            Vector3 p1 = new Vector3(transform.position.x, jumpPosition.y + 8, transform.position.z);
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
        

        private Vector3 GetEndPosition()
        {

            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = gameManager.GetTargetPosition();
            targetForce = forceFactor * (targetPosition - currentPosition).magnitude;

            Debug.Log("Target Force: " + targetForce + " Current Force: " + force);

            if (force > targetForce + maxForceThreshold)
            {
                return currentPosition;
            }
            
            return currentPosition + force / targetForce * (targetPosition - currentPosition);
        }

        private void OnCollisionStay(Collision other)
        {
            if (other.gameObject.CompareTag("Ground"))
            {
                gameManager.UpdateCurrentCube(other.gameObject.transform.parent.GetSiblingIndex());
                isOnGround = true;
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