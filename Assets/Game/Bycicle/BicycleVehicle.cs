using System;
using System.Collections;
using System.Collections.Generic;
using Game.Sensor;
using Game.Util;
using UnityEngine;
using Random = System.Random;

namespace Game.Bicycle
{
	public class BicycleVehicle : MonoBehaviour
	{
		public GameManager gameManager;
		public BicycleUIUpdater UIUpdater;
		private float maxSpeed = 30;
		public bool isUser;
		public TrackGenerator trackGenerator;
		
		public SensorManager sensorManager;
		public TurnController turnController;

		float horizontalInput = 0f;
		float vereticallInput = 0f;

		public Transform handle;
		bool braking;
		Rigidbody rb;

		public Vector3 COG;

		[SerializeField] float motorforce;
		[SerializeField] float brakeForce;
		float currentbrakeForce;

		float steeringAngle;
		[SerializeField] float currentSteeringAngle;
		[Range(0f, 0.1f)] [SerializeField] float speedteercontrolTime;
		[SerializeField] float maxSteeringAngle;
		[Range(0.000001f, 1)] [SerializeField] float turnSmoothing;

		[SerializeField] float maxlayingAngle = 45f;
		public float targetlayingAngle;
		[Range(-40, 40)] public float layingammount;
		[Range(0.000001f, 1)] [SerializeField] float leanSmoothing;

		[SerializeField] WheelCollider frontWheel;
		[SerializeField] WheelCollider backWheel;

		[SerializeField] Transform frontWheeltransform;
		[SerializeField] Transform backWheeltransform;

		[SerializeField] TrailRenderer fronttrail;
		[SerializeField] TrailRenderer rearttrail;

		public bool frontGrounded;
		public bool rearGrounded;

		private float speedFactor;

		// Start is called before the first frame update
		void Start()
		{
			StopEmitTrail();
			rb = GetComponent<Rigidbody>();
		}

		// Update is called once per frame
		void FixedUpdate()
		{
			
			if (gameManager.state == GameState.END)
			{
				rb.velocity = Vector3.zero;
					// Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime);
				rb.isKinematic = true;
			}
			
			if (gameManager.state == GameState.PLAY)
			{
				if (isUser) GetInput();
				else AutoDrive();
				
				HandleEngine();
				HandleSteering();
				UpdateWheels();
				UpdateHandle();
				LayOnTurn();
				DownPresureOnSpeed();
				EmitTrail();
				UpdateUI();
				gameManager.UpdatePlayerData(rb.velocity.magnitude, vereticallInput);
			}

		}

		private void UpdateUI()
		{
			// if (!isUser) return;
			float speed = Mathf.Clamp(rb.velocity.magnitude, 0, 50);
			UIUpdater.UpdateSpeedUI(speed, speed / maxSpeed);
			UIUpdater.UpdateForceUI(vereticallInput);
		}
		
		
		private void AutoDrive()
		{
			if (transform.position.z - trackGenerator.userbike.position.z > 100)
			{
				vereticallInput = Mathf.Min(0.1f, sensorManager.GetData(SensorPosition.LEFT));	
			}
			else if (transform.position.z - trackGenerator.userbike.position.z < -100)
			{
				vereticallInput = Mathf.Max(5f,sensorManager.GetData(SensorPosition.LEFT));
			}
			else
			{
				vereticallInput = UnityEngine.Random.Range(0.1f, 5f);
			}
			
			horizontalInput = Mathf.Clamp(turnController.horizontalInput / maxSteeringAngle, 0 , 1);
		}
		
		public void GetInput()
		{
			if (sensorManager.GetData(SensorPosition.LEFT) > 0.1f)
			{
				vereticallInput = sensorManager.GetData(SensorPosition.LEFT); // 0 to 5
				horizontalInput = Mathf.Clamp(turnController.horizontalInput / maxSteeringAngle, 0 , 1);
			}
			else
			{
				vereticallInput = 0;
				horizontalInput = 0;
			}
			// horizontalInput = -0.05f;
			// horizontalInput = Input.GetAxis("Horizontal");
			// vereticallInput = Input.GetAxis("Vertical");
			// braking = Input.GetKey(KeyCode.Space);
		}

		public void HandleEngine()
		{
			backWheel.motorTorque = vereticallInput * motorforce;
			currentbrakeForce = braking ? brakeForce : 0f;
			if (braking)
			{
				ApplyBraking();
			}
			else
			{
				ReleaseBrakibg();
			}
		}

		public void DownPresureOnSpeed()
		{
			Vector3 downforce = Vector3.down;
			float downpressure;
			if (rb.velocity.magnitude > 5)
			{
				downpressure = rb.velocity.magnitude;
				rb.AddForce(downforce * downpressure, ForceMode.Force);
			}

		}

		public void ApplyBraking()
		{
			//frontWheel.brakeTorque = currentbrakeForce/2;
			frontWheel.brakeTorque = currentbrakeForce;
			backWheel.brakeTorque = currentbrakeForce;
		}

		public void ReleaseBrakibg()
		{
			frontWheel.brakeTorque = 0;
			backWheel.brakeTorque = 0;
		}

		public void SpeedSteerinReductor()
		{
			if (rb.velocity.magnitude <
			    5) //We set the limiting factor for the steering thus allowing how much steer we give to the player in relation to the speed
			{
				maxSteeringAngle = Mathf.LerpAngle(maxSteeringAngle, 50, speedteercontrolTime);
			}

			if (rb.velocity.magnitude > 5 && rb.velocity.magnitude < 10)
			{
				maxSteeringAngle = Mathf.LerpAngle(maxSteeringAngle, 30, speedteercontrolTime);
			}

			if (rb.velocity.magnitude > 10 && rb.velocity.magnitude < 15)
			{
				maxSteeringAngle = Mathf.LerpAngle(maxSteeringAngle, 15, speedteercontrolTime);
			}

			if (rb.velocity.magnitude > 15 && rb.velocity.magnitude < 20)
			{
				maxSteeringAngle = Mathf.LerpAngle(maxSteeringAngle, 10, speedteercontrolTime);
			}

			if (rb.velocity.magnitude > 20)
			{
				maxSteeringAngle = Mathf.LerpAngle(maxSteeringAngle, 5, speedteercontrolTime);
			}
		}

		public void HandleSteering()
		{
			SpeedSteerinReductor();

			// currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, horizontalInput, turnSmoothing);
			// currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, maxSteeringAngle * horizontalInput, turnSmoothing);
			// frontWheel.steerAngle = currentSteeringAngle;

			//We set the target laying angle to the + or - input value of our steering 
			//We invert our input for rotating in the ocrrect axis
			targetlayingAngle = maxlayingAngle * -horizontalInput;
		}

		private void LayOnTurn()
		{
			Vector3 currentRot = transform.rotation.eulerAngles;

			if (rb.velocity.magnitude < 1)
			{
				layingammount = Mathf.LerpAngle(layingammount, 0f, 0.05f);
				transform.rotation = Quaternion.Euler(currentRot.x, currentRot.y, layingammount);
				return;
			}

			if (currentSteeringAngle < 0.5f && currentSteeringAngle > -0.5) //We're straight
			{
				layingammount = Mathf.LerpAngle(layingammount, 0f, leanSmoothing);
			}
			else //We're turning
			{
				layingammount = Mathf.LerpAngle(layingammount, targetlayingAngle, leanSmoothing);
				rb.centerOfMass = new Vector3(rb.centerOfMass.x, COG.y, rb.centerOfMass.z);
			}

			transform.rotation = Quaternion.Euler(currentRot.x, currentRot.y, layingammount);
		}

		public void UpdateWheels()
		{
			UpdateSingleWheel(frontWheel, frontWheeltransform);
			UpdateSingleWheel(backWheel, backWheeltransform);
		}

		public void UpdateHandle()
		{
			handle.localRotation = Quaternion.Euler(handle.localRotation.eulerAngles.x, frontWheel.steerAngle,
				handle.localRotation.eulerAngles.z);
		}

		private void EmitTrail()
		{
			frontGrounded = frontWheel.GetGroundHit(out WheelHit Fhit);
			rearGrounded = backWheel.GetGroundHit(out WheelHit Rhit);

			if (frontGrounded)
			{
				fronttrail.emitting = true;
			}
			else
			{
				fronttrail.emitting = false;
			}

			if (rearGrounded)
			{
				rearttrail.emitting = true;
			}
			else
			{
				rearttrail.emitting = false;
			}

			//fronttrail.emitting = true;
			//rearttrail.emitting = true;
		}

		private void StopEmitTrail()
		{
			fronttrail.emitting = false;
			rearttrail.emitting = false;
		}

		private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
		{
			Vector3 pos;
			Quaternion rot;
			wheelCollider.GetWorldPose(out pos, out rot);
			wheelTransform.rotation = rot;
			wheelTransform.position = pos;
		}


		// private void OnCollisionEnter(Collision other)
		// {
		// 	if (other.gameObject.CompareTag("Ground"))
		// 	{
		// 		Vector3 targetVelocityNormalized = other.gameObject.transform.rotation.eulerAngles.normalized;
		// 		horizontalInput = Vector3.Angle(rb.velocity.normalized, targetVelocityNormalized);
		// 		Debug.Log(horizontalInput + ", " + vereticallInput);
		// 	}
		//
		// }
	}
}
