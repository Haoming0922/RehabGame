using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Util;

namespace Game.Sensor
{
    public class WheelchairPair : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI guideText;
        public List<GameObject> pairingProgressBars;
        
        
        private void Start()
        {
            StartCoroutine(WheelchairPairCalibrate());
        }

        IEnumerator WheelchairPairCalibrate()
        {
            SensorPairingData sensorPairingData = new SensorPairingData(Exercise.WHEELCHAIR);

            guideText.text = "Please mount the sensors to the wheelchair, and roll both wheels to wake them...";
            yield return new WaitForSeconds(5);

            foreach (var progressBarObject in pairingProgressBars)
            {
                PairingProgressBar progressBar = progressBarObject.GetComponent<PairingProgressBar>();

                guideText.text = "Please only roll the " + progressBar.sensorPosition.ToString().ToLower() +
                                 " wheel forward";
                progressBar.SetProgressBarActive(true);

                while (!progressBar.IsFinished())
                {
                    yield return null;
                }

                guideText.text = "Pairing Success";
                progressBar.SetProgressBarActive(false);

                sensorPairingData.SetSensorAddress(progressBar.sensorPosition, progressBar.GetSensorAddress());
                sensorPairingData.SetSensorDirection(progressBar.sensorPosition, progressBar.GetRotationDirection());

                yield return new WaitForSeconds(1);
            }

            DataSaver.SaveData("Wheelchair.sensorpair", typeof(SensorPairingData));
            gameObject.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.Invoke();
        }



    }
}
