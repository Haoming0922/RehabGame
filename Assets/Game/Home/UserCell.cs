using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RehabDB;

public class UserCell : MonoBehaviour
{
    public TextMeshProUGUI patientName;  // Assign this in the inspector
    public Patient patient;
    
    public void OnButtonClick()
    {
        // Get the text from the button and store it in a variable
        // DBManager.Instance.currentPatient = patient;
        StartCoroutine(DBManager.Instance.LoadCurrentPatient(patientName.text));
    }
}
