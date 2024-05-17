using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Game.Util;
using UnityEngine.SceneManagement;

namespace Game.Wheelchair
{
    public class WheelchairUIUpdater2D : MonoBehaviour
    {
        public GameManager2D gameManager2D;
        public Game.Util.CircularProgressBar forceProgressBar;

        public GameObject startCanvas;
        public GameObject avatarCanvas;
        public GameObject evaluationCanvas;

        
        public void UpdateForceUI(float force)
        {
            forceProgressBar.UpdateFillAmount(force / 2);
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
            
            while (time < 1f)
            {
                time += Time.deltaTime;
                forceProgressBar.UpdateFillAmount(time);
                yield return null;
            }
            
            evaluationCanvas.SetActive(true);
            TimeSpan timeSpan = new TimeSpan((int)seconds / 60, (int)seconds % 60, (int)((seconds - (int)seconds) * 100));
            evaluationCanvas.transform.GetChild(0).GetChild(0).GetChild(0).Find("TimeText").GetComponent<TextMeshProUGUI>().text = timeSpan.ToString(@"hh\:mm\:ss");
            evaluationCanvas.transform.GetChild(0).GetChild(0).GetChild(0).Find("PerformanceText").GetComponent<TextMeshProUGUI>()
                .text = strength > 1 ? "+ " + strength.ToString("0.0") + "%" : "+ 0%";
            
            yield return new WaitForSeconds(15f);


            float move = gameManager2D.sensorManager.GetData(SensorPosition.LEFT);
            while (move < 10)
            {
                move += Mathf.Max(gameManager2D.sensorManager.GetData(SensorPosition.LEFT), gameManager2D.sensorManager.GetData(SensorPosition.RIGHT));
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