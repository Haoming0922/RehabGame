using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserCell : MonoBehaviour
{
    public TextMeshProUGUI patientName;  // Assign this in the inspector
    
    public void OnButtonClick()
    {
        // Get the text from the button and store it in a variable
        DBConnector.Instance.currentPatient = patientName.text;
        
    }
}
