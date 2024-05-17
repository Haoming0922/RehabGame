using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Game.Sensor;
using Game.Util;

public class PairingProgressBar : MonoBehaviour
{
	public SensorPosition sensorPosition;
	private string sensorAddress = null;
	private RotationDirection direction = RotationDirection.NULL;
	
	private bool isActive = false;
	private bool isFinished = false;
	
	private float rotationX = 0;
	private float rotationY = 0;
	private float rotationZ = 0;
	
	[Header("Progress Bar")]
	[SerializeField] private Color m_MainColor = Color.white;
	[SerializeField] private Color m_FillColor = Color.green;
	[SerializeField] private int m_NumberOfSegments = 5;
	[Range(0, 360)] [SerializeField] private float m_StartAngle = 40;
	[Range(0, 360)] [SerializeField] private float m_EndAngle = 320;
	[SerializeField] private float m_SizeOfNotch = 5;
	[Range(0, 1f)] private float m_FillAmount = 0.0f;
	private Image m_Image;
	private List<Image> m_ProgressToFill = new List<Image> ();
	private float m_SizeOfSegment;
	

    private void Awake()
    {
	    SetProgressBar();		
    }
	
	private void Update()
	{
		ProgressBarUpdate();
	}

	public string GetSensorAddress()
	{
		return sensorAddress;
	}
	public RotationDirection GetRotationDirection()
	{
		return direction;
	}

	#region Progress Bar

	
	private void OnDisable()
	{
		SyncsenseSensorManager.OnSensorDataReceivedEvent -= ProgressGrowEvent;
	}
	
	private void OnDestroy()
	{
		SyncsenseSensorManager.OnSensorDataReceivedEvent -= ProgressGrowEvent;
	}

	public void SetProgressBarActive(bool active)
	{
		isActive = active;
		if (active)
		{
			SyncsenseSensorManager.OnSensorDataReceivedEvent += ProgressGrowEvent;
		}
		else
		{
			SyncsenseSensorManager.OnSensorDataReceivedEvent -= ProgressGrowEvent;
		}
	}
	
	private void SetProgressBar()
	{
		// Get images in Children
		m_Image = GetComponentInChildren<Image>();
		m_Image.color = m_MainColor;
		m_Image.gameObject.SetActive(false);

		// Calculate notches
		float startNormalAngle = NormalizeAngle(m_StartAngle);
		float endNormalAngle = NormalizeAngle(360 - m_EndAngle);
		float notchesNormalAngle = (m_NumberOfSegments - 1) * NormalizeAngle(m_SizeOfNotch);
		float allSegmentsAngleArea = 1 - startNormalAngle - endNormalAngle - notchesNormalAngle;
		
		// Count size of segments
		m_SizeOfSegment = allSegmentsAngleArea / m_NumberOfSegments;
		for (int i = 0; i < m_NumberOfSegments; i++) {
			GameObject currentSegment = Instantiate(m_Image.gameObject, transform.position, Quaternion.identity, transform);
			currentSegment.SetActive(true);

			Image segmentImage = currentSegment.GetComponent<Image>();
			segmentImage.fillAmount = m_SizeOfSegment;

			Image segmentFillImage = segmentImage.transform.GetChild (0).GetComponent<Image> ();
			segmentFillImage.color = m_MainColor;
			m_ProgressToFill.Add (segmentFillImage);

			float zRot = m_StartAngle + i * ConvertCircleFragmentToAngle(m_SizeOfSegment) + i * m_SizeOfNotch;
			segmentImage.transform.rotation = Quaternion.Euler(0,0, -zRot);
		}
	}
	
	private float NormalizeAngle(float angle) {
		return Mathf.Clamp01(angle / 360f);
	}

	private float ConvertCircleFragmentToAngle(float fragment) {
		return 360 * fragment;
	}
	
	private void ProgressBarUpdate()
	{
		Debug.Log("[Haoming] fill: " + m_FillAmount);
		if (isActive)
		{
			for (int i = 0; i < m_NumberOfSegments; i++)
			{
				m_ProgressToFill [i].color = m_FillColor;
				m_ProgressToFill [i].fillAmount = (m_FillAmount * ((m_EndAngle-m_StartAngle)/360)) - m_SizeOfSegment * i;
			}
			if (m_FillAmount > 0.99f)
			{
				SetRotationDirection();
				isFinished = true;
			}
		}
	}

	private void ProgressGrowEvent(SensorDataReceived data)
	{
		if (Calculation.IsMove(data))
		{
			if (data.deviceAddress == sensorAddress || sensorAddress == null)
			{
				sensorAddress = data.deviceAddress;
				
				m_FillAmount += 0.01f;
				AddRotation(data);
			}
			else
			{
				sensorAddress = data.deviceAddress;
				m_FillAmount = 0;
				ResetRotation();
			} 
		}
	}
	
	public bool IsFinished()
	{
		return isFinished;
	}
	
	#endregion
	

	#region Handle Rotation Direction

	private void AddRotation(SensorDataReceived data)
	{
		rotationX += data.gyroX;
		rotationY += data.gyroY;
		rotationZ += data.gyroZ;
	}
	
	private void SetRotationDirection()
	{
		if(Mathf.Abs(rotationX) > Mathf.Abs(rotationY))
		{
			if(Mathf.Abs(rotationX) > Mathf.Abs(rotationZ)) // X
			{
				direction = rotationX > 0 ? RotationDirection.XPOSITIVE : RotationDirection.XNEGATIVE;
			}
			else // Z
			{
				direction = rotationZ > 0 ? RotationDirection.ZPOSITIVE : RotationDirection.ZNEGATIVE;
			}
		}
		else
		{
			if (Mathf.Abs(rotationY) > Mathf.Abs(rotationZ)) // Y
			{
				direction = rotationY > 0 ? RotationDirection.YPOSITIVE : RotationDirection.YNEGATIVE;
			}
			else // Z
			{
				direction = rotationZ > 0 ? RotationDirection.ZPOSITIVE : RotationDirection.ZNEGATIVE;
			}
		}
	}
    
	private void ResetRotation()
	{
		rotationX = 0;
		rotationY = 0;
		rotationZ = 0;
	}

	#endregion
	
}
