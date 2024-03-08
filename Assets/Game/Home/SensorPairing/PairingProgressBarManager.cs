using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Core;

public class PairingProgressBarManager : MonoBehaviour
{
    public Exercise exercise;
    [SerializeField] private TextMeshProUGUI guideText;
    public List<GameObject> pairingProgressBars;
    
    private SensorPairingData _sensorPairingData;
    
    
    private void Start()
    {
        _sensorPairingData = new SensorPairingData(exercise);
        switch (exercise)
        {
            case Exercise.WHEELCHAIR:
                StartCoroutine(WheelchairSensorPairing());
                break;
            case Exercise.DUMBBELL:
                StartCoroutine(DumbbelSensorPairing());
                break;
            case Exercise.CYCLE:
                StartCoroutine(CyclingSensorPairing());
                break;
            default: break;
        }
    }

    IEnumerator WheelchairSensorPairing()
    {
        guideText.text = "Please mount the sensors to the wheelchair, and roll both wheels to wake them...";
        yield return new WaitForSeconds(5);

        foreach (var progressBarObject in pairingProgressBars)
        {
            PairingProgressBar progressBar = progressBarObject.GetComponent<PairingProgressBar>();
            
            guideText.text = "Please only roll the " + progressBar.sensorPosition.ToString().ToLower() + " wheel forward";
            progressBar.SetProgressBarActive(true);
            
            while (!progressBar.IsFinished())
            {
                yield return null;
            }
            
            guideText.text = "Pairing Success";
            progressBar.SetProgressBarActive(false);
            
            _sensorPairingData.SetSensorAddress(progressBar.sensorPosition, progressBar.GetSensorAddress());
            _sensorPairingData.SetSensorDirection(progressBar.sensorPosition, progressBar.GetRotationDirection());

            yield return new WaitForSeconds(1);
        }
        
        _sensorPairingData.SaveData();
        gameObject.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.Invoke();
    }

    IEnumerator DumbbelSensorPairing()
    {
        guideText.text = "Please move the sensors to wake them...";
        yield return new WaitForSeconds(5);

        foreach (var progressBarObject in pairingProgressBars)
        {
            PairingProgressBar progressBar = progressBarObject.GetComponent<PairingProgressBar>();
            
            guideText.text = "Please only move the " + progressBar.sensorPosition.ToString().ToLower() + " sensor";
            progressBar.SetProgressBarActive(true);
            
            while (!progressBar.IsFinished())
            {
                yield return null;
            }
            
            guideText.text = "Pairing Success";
            progressBar.SetProgressBarActive(false);
            
            _sensorPairingData.SetSensorAddress(progressBar.sensorPosition, progressBar.GetSensorAddress());
            _sensorPairingData.SetSensorDirection(progressBar.sensorPosition, RotationDirection.NULL);

            yield return new WaitForSeconds(1);
        }
        
        _sensorPairingData.SaveData();
        gameObject.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.Invoke();
    }

    IEnumerator CyclingSensorPairing() 
    {
        guideText.text = "Please move the sensors to wake them..."; //TODO
        yield return new WaitForSeconds(5);

        foreach (var progressBarObject in pairingProgressBars)
        {
            PairingProgressBar progressBar = progressBarObject.GetComponent<PairingProgressBar>();
            
            guideText.text = "Please only move the " + progressBar.sensorPosition.ToString().ToLower() + " sensor";
            progressBar.SetProgressBarActive(true);
            
            while (!progressBar.IsFinished())
            {
                yield return null;
            }
            
            guideText.text = "Pairing Success";
            progressBar.SetProgressBarActive(false);
            
            _sensorPairingData.SetSensorAddress(progressBar.sensorPosition, progressBar.GetSensorAddress());
            _sensorPairingData.SetSensorDirection(progressBar.sensorPosition, RotationDirection.NULL);

            yield return new WaitForSeconds(1);
        }
        
        _sensorPairingData.SaveData();
        gameObject.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.Invoke();
    }
        
}
