using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MappingManager : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI guideText;
	public List<GameObject> mappingList;

	private void Awake()
	{
		guideText.text = "Please roll both wheels forward to start...";
		StartCoroutine(MapingSensor());
	}
	
	IEnumerator MapingSensor()
	{
		yield return new WaitForSeconds(3);
		for (int mappingIndex = 0; mappingIndex < mappingList.Count; mappingIndex++)
		{
			MappingProgressBar progressBar = mappingList[mappingIndex].GetComponent<MappingProgressBar>();
			guideText.text = "Please only roll the " + progressBar.gameController.ToLower() + " wheel forward";
			progressBar.SetProgressBarActive(true);
			while (!progressBar.IsFinished())
			{
				yield return null;
			}
			progressBar.SetProgressBarActive(false);
			guideText.text = "Pairing Success";
			yield return new WaitForSeconds(1);
		}
		SceneManager.LoadScene("WheelChair");
	}
	

}