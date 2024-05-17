using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Game.Util;
using UnityEngine.SceneManagement;

namespace Game.Bicycle
{
    public class BicycleUIUpdater : MonoBehaviour
    {
        public GameManager gameManager;
        public Game.Util.CircularProgressBar  speedometerProgressBar;
        public TextMeshProUGUI speedText;
        public Game.Util.CircularProgressBar  forceProgressBar;
        public TextMeshProUGUI forceText;
        
        public GameObject startCanvas;
        public GameObject avatarCanvas;
        public GameObject journeyCanvas;
        public GameObject evaluationCanvas;
        
        public void UpdateSpeedUI(float speed, float speedometer)
        {
            speedometerProgressBar.UpdateFillAmount(speedometer);
            speedText.text = (int) speed + "km/h";
        }
        
        public void UpdateForceUI(float force)
        {
            forceProgressBar.UpdateFillAmount(force / 5);
            forceText.text = force.ToString("0.0") + " force";
        }


        public void ShowMoveToStartUI()
        {
            startCanvas.transform.GetChild(1).gameObject.SetActive(false);
            startCanvas.transform.GetChild(0).gameObject.SetActive(true);
            TextMeshProUGUI textStart = startCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            textStart.text = "Move to be Ready";
            textStart.fontSize *= 0.7f;
            // startCanvas.transform.GetChild(1).gameObject.SetActive(true);
        }

        public IEnumerator ShowGameStartUI()
        {
            startCanvas.transform.GetChild(1).gameObject.SetActive(false);
            startCanvas.transform.GetChild(0).gameObject.SetActive(true);
            TextMeshProUGUI textStart = startCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            textStart.fontSize /= 0.7f;
            textStart.text = "READY";
            yield return new WaitForSeconds(2);
            textStart.text = "3";
            yield return new WaitForSeconds(1);
            textStart.text = "2";
            yield return new WaitForSeconds(1);
            textStart.text = "1";
            yield return new WaitForSeconds(1);
            textStart.text = "GO";
            yield return new WaitForSeconds(0.5f);
            startCanvas.SetActive(false);
        }

        public IEnumerator ShowGameEndUI(float seconds, float speed, float strength, int coins)
        {
            float time = 0;
            // Vector3 avatarNewPosition = avatarCanvas.transform.position + avatarCanvas.transform.right * 2;
            // Vector3 journeyNewPosition = journeyCanvas.transform.position - journeyCanvas.transform.right * 2;
            
            journeyCanvas.transform.GetChild(2).gameObject.SetActive(false);
            
            while (time < 1f)
            {
                time += Time.deltaTime;
                // avatarCanvas.transform.position = Vector3.Lerp(avatarCanvas.transform.position, avatarNewPosition, Time.deltaTime);
                // journeyCanvas.transform.position = Vector3.Lerp(journeyCanvas.transform.position, journeyNewPosition, Time.deltaTime);
                forceProgressBar.UpdateFillAmount(time);
                speedometerProgressBar.UpdateFillAmount(time);
                yield return null;
            }
            
            strength = (strength - 1) * 100;
            evaluationCanvas.SetActive(true);
            // evaluationCanvas.transform.GetChild(0).GetChild(0).GetChild(0).Find("TimeText").GetComponent<TextMeshProUGUI>().text = (int) seconds / 60 + ":" + (int) seconds % 60 + ":" +
            // (int)((seconds - (int)seconds) * 100);
            TimeSpan timeSpan = new TimeSpan((int)seconds / 60, (int)seconds % 60, (int)((seconds - (int)seconds) * 100));
            evaluationCanvas.transform.GetChild(0).GetChild(0).GetChild(0).Find("TimeText").GetComponent<TextMeshProUGUI>().text = timeSpan.ToString(@"hh\:mm\:ss");
            evaluationCanvas.transform.GetChild(0).GetChild(0).GetChild(0).Find("SpeedText").GetComponent<TextMeshProUGUI>().text =
                speed.ToString("0.0") + " KM/H";
            evaluationCanvas.transform.GetChild(0).GetChild(0).GetChild(0).Find("PerformanceText").GetComponent<TextMeshProUGUI>()
                .text = strength > 0 ? "+ " + strength.ToString("0.0") + "%" : "+ 0%";
            // evaluationCanvas.transform.GetChild(0).GetChild(0).GetChild(0).Find("CoinsText").GetComponent<TextMeshProUGUI>().text =
            //     "+ " + coins;
            
            yield return new WaitForSeconds(15f);

            float move = gameManager.sensorManager.GetData(SensorPosition.LEFT);
            while (move < 20)
            {
                move += gameManager.sensorManager.GetData(SensorPosition.LEFT);
                yield return null;
            }

            SceneManager.LoadScene("Home");
            
        }
        
        public void GoHome()
        {
            SceneManager.LoadScene("Home");
        }
        
    }
}