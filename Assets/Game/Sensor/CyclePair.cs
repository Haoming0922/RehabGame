using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Util;

namespace Game.Sensor
{
    public class CyclePair : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI guideText;
        public List<GameObject> pairingProgressBars;
        
        
        private void Start()
        {
            StartCoroutine(CyclePairCalibrate());
        }

        IEnumerator CyclePairCalibrate()
        {
            SensorPairingData sensorPairingData = new SensorPairingData(Exercise.Cycle);

            guideText.text = "Please mount one sensor to the hand cycler, and roll to wake it...";
            yield return new WaitForSeconds(5);

            foreach (var progressBarObject in pairingProgressBars)
            {
                PairingProgressBar progressBar = progressBarObject.GetComponent<PairingProgressBar>();

                guideText.text = "Please roll the hand cycler";
                progressBar.SetProgressBarActive(true);

                while (!progressBar.IsFinished())
                {
                    yield return null;
                }

                guideText.text = "Pairing Success";
                progressBar.SetProgressBarActive(false);

                sensorPairingData.SetSensorAddress(progressBar.sensorPosition, progressBar.GetSensorAddress());

                yield return new WaitForSeconds(1);
            }

            DataSaver.SaveData("Cycle.sensorpair", sensorPairingData);
            gameObject.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.Invoke();
        }



    }
}
