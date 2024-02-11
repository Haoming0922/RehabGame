using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Mapping : MonoBehaviour
{
	public List<GameObject> mappingList;
	private bool isRunning = false;
	private int mappingIndex = 0;
	private void Update() {
		// if(SensorDataManager.connectedDevices >= 2 && !isRunning) StartCoroutine(MapingSensor());
	}

	
	
	// IEnumerator MapingSensor()
	// {
	// 	isRunning = true;
	// 	mappingList[mappingIndex].GetComponent<MappingProgressBar>().SetActive(true);
	// 	SceneManager.LoadScene("WheelChair");
	// }
	

}