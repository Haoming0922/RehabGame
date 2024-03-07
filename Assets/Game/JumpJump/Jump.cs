using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.JumpJump
{

    public class Jump : MonoBehaviour
    {
        public Rigidbody player;
        public GameObject curve;
        public Transform cam;

        private GameObject mass;

        public float maxThreshold;
        private float force = 0;
        private float targetForce = 0;
        private int currentCube = 0;
        private int sphereNumber = 0;

        private bool isOnGround = false;

        void Start()
        {
            Application.targetFrameRate = -1;
            mass = transform.GetChild(0).gameObject;
            SetMass();
        }

        void FixedUpdate()
        {
            if (currentCube == LevelGenerator.cubes.Count - 1)
            {
                Debug.Log("Success");
                return;
            }

            StartCoroutine(SlowerControl());
        }

        private IEnumerator SlowerControl()
        {
            Vector3 jumpPosition = GetEndPosition(force);
            if (Input.GetKey(KeyCode.J) && isOnGround)
            {
                force++;
                if (jumpPosition != transform.position) sphereNumber = DrawCurve(jumpPosition);
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                if (force > 0)
                {
                    StartCoroutine(MoveAlongCurve(jumpPosition));
                    force = 0;
                }
            }
        }


        private IEnumerator MoveAlongCurve(Vector3 jumpPosition)
        {
            for (int i = 0; i < sphereNumber; i++)
            {
                curve.transform.GetChild(i).gameObject.SetActive(false);
            }

            // Bezier Curve
            Vector3 p0 = transform.position;
            Vector3 p1 = new Vector3(transform.position.x, jumpPosition.y + 6, transform.position.z);
            Vector3 p2 = new Vector3(jumpPosition.x, jumpPosition.y + 4, jumpPosition.z + 2);
            Vector3 p3 = jumpPosition;

            Vector3 rotationAngle = LevelGenerator.cubes[currentCube + 1].transform.rotation.eulerAngles -
                                    LevelGenerator.cubes[currentCube].transform.rotation.eulerAngles;

            //this.GetComponent<Rigidbody>().useGravity = false;

            float t = 0;
            while (t < 1)
            {
                Vector3 bt = Mathf.Pow(1 - t, 3) * p0 +
                             3 * t * Mathf.Pow(1 - t, 2) * p1 +
                             3 * (1 - t) * Mathf.Pow(t, 2) * p2 +
                             Mathf.Pow(t, 3) * p3;
                transform.position = bt;
                //transform.Rotate(rotationAngle * Time.deltaTime);
                t += 1f * Time.deltaTime;
                yield return null;
            }

            //this.GetComponent<Rigidbody>().useGravity = true;
        }

        private void SetMass()
        {
            player.centerOfMass = Vector3.down;
            mass.transform.position = transform.position + player.centerOfMass;
        }


        private int DrawCurve(Vector3 jumpPosition)
        {
            // Bezier Curve
            Vector3 p0 = transform.position;
            Vector3 p1 = new Vector3(transform.position.x, jumpPosition.y + 6, transform.position.z);
            Vector3 p2 = new Vector3(jumpPosition.x, jumpPosition.y + 4, jumpPosition.z + 2);
            Vector3 p3 = jumpPosition;

            int totalSphere = curve.transform.childCount;
            Transform lastSphere = null;

            for (int i = 0; i < totalSphere; i++)
            {
                float t = 0.15f * i;
                Vector3 bt = Mathf.Pow(1 - t, 3) * p0 +
                             3 * t * Mathf.Pow(1 - t, 2) * p1 +
                             3 * (1 - t) * Mathf.Pow(t, 2) * p2 +
                             Mathf.Pow(t, 3) * p3;

                curve.transform.GetChild(i).gameObject.SetActive(true);
                curve.transform.GetChild(i).position = bt;
                lastSphere = curve.transform.GetChild(i);

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

            return totalSphere;
        }


        private bool EnoughSpace(Vector3 p1, Vector3 p2)
        {
            return (p1 - p2).magnitude > 0.2 ? true : false;
        }


        private Vector3 GetEndPosition(float force)
        {

            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = LevelGenerator.cubes[currentCube + 1].transform.position;
            targetPosition.y += LevelGenerator.cubes[currentCube + 1].transform.lossyScale.y;
            targetForce = 5 * (targetPosition - currentPosition).magnitude;

            if (force > targetForce + maxThreshold) return currentPosition;

            if (Mathf.Abs(force - targetForce) <= 5) force = targetForce;
            return currentPosition + force / targetForce * (targetPosition - currentPosition);
        }

        private void OnCollisionStay(Collision other)
        {
            if (other.gameObject.CompareTag("Ground"))
            {
                currentCube = other.gameObject.transform.parent.GetSiblingIndex();
                isOnGround = true;
                //cam.LookAt(transform.position);
                Debug.Log(currentCube);
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
                transform.position = LevelGenerator.cubes[currentCube].transform.position +
                                     new Vector3(0, LevelGenerator.cubes[currentCube].transform.lossyScale.y, 0) +
                                     1 * transform.up;
            }
        }

        public Vector3 GetLookAt()
        {
            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = LevelGenerator.cubes[currentCube + 1].transform.position;
            targetPosition.y += LevelGenerator.cubes[currentCube + 1].transform.lossyScale.y;
            return (currentPosition + targetPosition) / 2;
        }
    }

}