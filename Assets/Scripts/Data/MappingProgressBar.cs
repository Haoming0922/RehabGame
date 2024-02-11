using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MappingProgressBar : MonoBehaviour
{
[Header("Colors")]
	[SerializeField] private Color m_MainColor = Color.white;
	[SerializeField] private Color m_FillColor = Color.green;
	
	[Header("General")]
	[SerializeField] private int m_NumberOfSegments = 5;
	[Range(0, 360)] [SerializeField] private float m_StartAngle = 40;
	[Range(0, 360)] [SerializeField] private float m_EndAngle = 320;
	[SerializeField] private float m_SizeOfNotch = 5;
	[Range(0, 1f)] [SerializeField] private float m_FillAmount = 0.0f;

	[Header("General")]
	[SerializeField] private TextMeshProUGUI text;
	public string gameController;
	
	private Image m_Image;
	private List<Image> m_ProgressToFill = new List<Image> ();
	private float m_SizeOfSegment;

	private string currentMapping = null;

	private float rotationX = 0;
    private float rotationY = 0;
    private float rotationZ = 0;

    private bool isActive = false;

    private void Awake()
    {
	    SetProgressBar();
	    SubscribeDataEvent();
    }


	void SetProgressBar()
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
			segmentFillImage.color = m_FillColor;
			m_ProgressToFill.Add (segmentFillImage);

			float zRot = m_StartAngle + i * ConvertCircleFragmentToAngle(m_SizeOfSegment) + i * m_SizeOfNotch;
			segmentImage.transform.rotation = Quaternion.Euler(0,0, -zRot);
		}
	}

	void SubscribeDataEvent()
	{
		SyncsenseSensorManager.OnSensorDataReceivedEvent -= HandleMovingSensorData;
		SyncsenseSensorManager.OnSensorDataReceivedEvent += HandleMovingSensorData;
	}
	
	private void Update() {
		if (isActive)
		{
			for (int i = 0; i < m_NumberOfSegments; i++) {
				m_ProgressToFill [i].fillAmount = (m_FillAmount * ((m_EndAngle-m_StartAngle)/360)) - m_SizeOfSegment * i;
			}
			if (m_FillAmount > 0.99f)
			{
				if (currentMapping != null && !GameDataManager.Instance.sensorMapping.ContainsKey(currentMapping))
				{
					GameDataManager.Instance.sensorMapping.Add(currentMapping, gameController);
					SetRotation(currentMapping);
					isActive = false;
				}
			}
		}
	}

	private void OnDestroy()
	{
		SyncsenseSensorManager.OnSensorDataReceivedEvent -= HandleMovingSensorData;
	}

	public void SetActive(bool active)
	{
		isActive = active;
	}
	
	private float NormalizeAngle(float angle) {
		return Mathf.Clamp01(angle / 360f);
	}

	private float ConvertCircleFragmentToAngle(float fragment) {
		return 360 * fragment;
	}

	public void HandleMovingSensorData(SensorDataReceived data)
	{
		float motion = Calculation.AverageMotion(data);

		if (GameDataManager.Instance.sensorMapping.ContainsKey(data.deviceAddress)) return; // already mapped
		
		if (motion > 15f)
		{
			if (data.deviceAddress == currentMapping || currentMapping == null)
			{
				currentMapping = data.deviceAddress;
				
				m_FillAmount += 0.01f;
				AddRotation(data);
            }
			else
			{
				currentMapping = data.deviceAddress;
				m_FillAmount = 0;
				ResetRotation();
            }
			
			
		}
	}

	private void AddRotation(SensorDataReceived data)
	{
		rotationX += data.gyroX;
        rotationY += data.gyroY;
        rotationZ += data.gyroZ;
    }

    private void SetRotation(string deviceAddress)
    {
	    RotationType rotation;
        if(Mathf.Abs(rotationX) > Mathf.Abs(rotationY))
		{
			if(Mathf.Abs(rotationX) > Mathf.Abs(rotationZ)) // X
			{
				rotation = rotationX > 0 ? RotationType.XPositive : RotationType.XNegative;
			}
			else // Z
			{
                rotation = rotationZ > 0 ? RotationType.ZPositive : RotationType.ZNegative;
			}
        }
		else
		{
            if (Mathf.Abs(rotationY) > Mathf.Abs(rotationZ)) // Y
            {
                rotation = rotationY > 0 ? RotationType.YPositive : RotationType.YNegative;
            }
            else // Z
            {
                rotation = rotationZ > 0 ? RotationType.ZPositive : RotationType.ZNegative;
            }
        }
        GameDataManager.Instance.SetRotationCalibration(deviceAddress, rotation);
    }

    private void ResetRotation()
    {
		rotationX = 0;
        rotationY = 0;
        rotationZ = 0;
    }
}
