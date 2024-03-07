// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;
//
// public class MappingProgressBar : MonoBehaviour
// {
// 	[Header("Guide")]
// 	[SerializeField] private string controlType;
// 	public string gameController;
// 	[SerializeField] private Text text;
// 	
// 	[Header("Colors")]
// 	[SerializeField] private Color m_MainColor = Color.white;
// 	[SerializeField] private Color m_FillColor = Color.green;
// 	
// 	[Header("General")]
// 	[SerializeField] private int m_NumberOfSegments = 5;
// 	[Range(0, 360)] [SerializeField] private float m_StartAngle = 40;
// 	[Range(0, 360)] [SerializeField] private float m_EndAngle = 320;
// 	[SerializeField] private float m_SizeOfNotch = 5;
// 	[Range(0, 1f)] private float m_FillAmount = 0.0f;
// 	
// 	private Image m_Image;
// 	private List<Image> m_ProgressToFill = new List<Image> ();
// 	private float m_SizeOfSegment;
//
// 	private string currentMapping = null;
//
// 	private float rotationX = 0;
//     private float rotationY = 0;
//     private float rotationZ = 0;
//
//     private bool isActive = false;
//     private bool isFinished = false;
//
//     private void Awake()
//     {
// 	    SetProgressBar();		
//     }
// 	
// 	private void Update() {
// 		if (isActive)
// 		{
// 			for (int i = 0; i < m_NumberOfSegments; i++)
// 			{
// 				// text.text = gameController + m_FillAmount;
// 				m_ProgressToFill [i].color = m_FillColor;
// 				m_ProgressToFill [i].fillAmount = (m_FillAmount * ((m_EndAngle-m_StartAngle)/360)) - m_SizeOfSegment * i;
// 			}
// 			if (m_FillAmount > 0.99f)
// 			{
// 				if (GameDataManager.Instance.sensorMapping.TryAdd(currentMapping, gameController))
// 				{
// 					// GameDataManager.sensorAdded?.Invoke(gameController, currentMapping);
// 					SetData(currentMapping);
// 					isFinished = true;
// 				}
// 			}
// 		}
// 	}
//
// 	private void OnDestroy()
// 	{
// 		SyncsenseSensorManager.OnSensorDataReceivedEvent -= ProgressGrow;
// 	}
//
// 	public void SetProgressBarActive(bool active)
// 	{
// 		isActive = active;
// 		if (active)
// 		{
// 			SyncsenseSensorManager.OnSensorDataReceivedEvent += ProgressGrow;
// 		}
// 		else
// 		{
// 			SyncsenseSensorManager.OnSensorDataReceivedEvent -= ProgressGrow;
// 		}
// 	}
// 	
// 	public bool IsFinished()
// 	{
// 		return isFinished;
// 	}
// 	
// 	
// 	void SetProgressBar()
// 	{
// 		// Get images in Children
// 		m_Image = GetComponentInChildren<Image>();
// 		m_Image.color = m_MainColor;
// 		m_Image.gameObject.SetActive(false);
//
// 		// Calculate notches
// 		float startNormalAngle = NormalizeAngle(m_StartAngle);
// 		float endNormalAngle = NormalizeAngle(360 - m_EndAngle);
// 		float notchesNormalAngle = (m_NumberOfSegments - 1) * NormalizeAngle(m_SizeOfNotch);
// 		float allSegmentsAngleArea = 1 - startNormalAngle - endNormalAngle - notchesNormalAngle;
// 		
// 		// Count size of segments
// 		m_SizeOfSegment = allSegmentsAngleArea / m_NumberOfSegments;
// 		for (int i = 0; i < m_NumberOfSegments; i++) {
// 			GameObject currentSegment = Instantiate(m_Image.gameObject, transform.position, Quaternion.identity, transform);
// 			currentSegment.SetActive(true);
//
// 			Image segmentImage = currentSegment.GetComponent<Image>();
// 			segmentImage.fillAmount = m_SizeOfSegment;
//
// 			Image segmentFillImage = segmentImage.transform.GetChild (0).GetComponent<Image> ();
// 			segmentFillImage.color = m_MainColor;
// 			m_ProgressToFill.Add (segmentFillImage);
//
// 			float zRot = m_StartAngle + i * ConvertCircleFragmentToAngle(m_SizeOfSegment) + i * m_SizeOfNotch;
// 			segmentImage.transform.rotation = Quaternion.Euler(0,0, -zRot);
// 		}
// 	}
// 	
// 	private float NormalizeAngle(float angle) {
// 		return Mathf.Clamp01(angle / 360f);
// 	}
//
// 	private float ConvertCircleFragmentToAngle(float fragment) {
// 		return 360 * fragment;
// 	}
//
// 	void ProgressGrow(SensorDataReceived data)
// 	{
// 		float motion = Calculation.AverageMotion(data);
//
// 		if (GameDataManager.Instance.sensorMapping.ContainsKey(data.deviceAddress)) return; // already mapped
// 		
// 		if (motion > 15f)
// 		{
// 			if (data.deviceAddress == currentMapping || currentMapping == null)
// 			{
// 				currentMapping = data.deviceAddress;
// 				
// 				m_FillAmount += 0.01f;
// 				HandleData(data);
//             }
// 			else
// 			{
// 				currentMapping = data.deviceAddress;
// 				m_FillAmount = 0;
// 				ResetData();
//             }
// 			
// 			
// 		}
// 	}
//
//
//
// 	private void HandleData(SensorDataReceived data)
// 	{
// 		if (controlType == "Rotation")
// 		{
// 			AddRotation(data);
// 		}
// 	}
// 	
// 	private void SetData(string deviceAddress)
// 	{
// 		if (controlType == "Rotation")
// 		{
// 			SetRotation(deviceAddress);
// 		}
// 	}
//
// 	private void ResetData()
// 	{
// 		if (controlType == "Rotation")
// 		{
// 			ResetRotation();
// 		}
// 	}
//
// 	
// 	private void AddRotation(SensorDataReceived data)
// 	{
// 		rotationX += data.gyroX;
//         rotationY += data.gyroY;
//         rotationZ += data.gyroZ;
//     }
// 	
//
//     private void SetRotation(string deviceAddress)
//     {
// 	    EnumTypes rotation;
//         if(Mathf.Abs(rotationX) > Mathf.Abs(rotationY))
// 		{
// 			if(Mathf.Abs(rotationX) > Mathf.Abs(rotationZ)) // X
// 			{
// 				rotation = rotationX > 0 ? EnumTypes.XPositive : EnumTypes.XNegative;
// 			}
// 			else // Z
// 			{
//                 rotation = rotationZ > 0 ? EnumTypes.ZPositive : EnumTypes.ZNegative;
// 			}
//         }
// 		else
// 		{
//             if (Mathf.Abs(rotationY) > Mathf.Abs(rotationZ)) // Y
//             {
//                 rotation = rotationY > 0 ? EnumTypes.YPositive : EnumTypes.YNegative;
//             }
//             else // Z
//             {
//                 rotation = rotationZ > 0 ? EnumTypes.ZPositive : EnumTypes.ZNegative;
//             }
//         }
//         GameDataManager.Instance.SetRotationCalibration(deviceAddress, rotation);
//     }
//
//     
//     private void ResetRotation()
//     {
// 		rotationX = 0;
//         rotationY = 0;
//         rotationZ = 0;
//     }
// }
