using System;
using System.Collections;
using System.Collections.Generic;
using Game.Util;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.JumpJump
{
    public class JumpUIUpdater : MonoBehaviour
    {
        public GameManager gameManager;
        
        public Game.Util.CircularProgressBar  leftForceProgressBar;
        public Game.Util.CircularProgressBar  rightForceProgressBar;
        
        public GameObject leftAvatarCanvas;
        public GameObject rightAvatarCanvas;

        public GameObject startCanvas;
        public GameObject evaluationCanvas;

        private TextMeshProUGUI mainText;

        private void Start()
        {
            mainText = startCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }

        public void UpdateForceUI(float force)
        {
            leftForceProgressBar.UpdateFillAmount(force / 1000);
            // switch (position)
            // {
            //     case SensorPosition.LEFT:
            //         leftForceProgressBar.UpdateFillAmount(force);
            //         break;
            //     case SensorPosition.RIGHT:
            //         rightForceProgressBar.UpdateFillAmount(force);
            //         break;
            // }
        }


        public void DisplayOneSide()
        {
            leftAvatarCanvas.SetActive(true);
            // startCanvas.SetActive(true);
            // mainText.text = "Hæv Armene";
            
            // switch (position)
            // {
            //     case SensorPosition.LEFT:
            //         leftAvatarCanvas.SetActive(true);
            //         rightAvatarCanvas.SetActive(false);
            //         startCanvas.SetActive(true);
            //         mainText.text = "RAISE LEFT ARM";
            //         break;
            //     case SensorPosition.RIGHT:
            //         leftAvatarCanvas.SetActive(false);
            //         rightAvatarCanvas.SetActive(true);
            //         startCanvas.SetActive(true);
            //         mainText.text = "RAISE RIGHT ARM";
            //         break;
            // }
            
        }
        
        public void DisplayJumpUI(bool display)
        {
            leftAvatarCanvas.SetActive(display);
            // rightAvatarCanvas.SetActive(display);
            // startCanvas.SetActive(display);
        }

        public void ShowMoveToStartUI()
        {
            startCanvas.transform.GetChild(1).gameObject.SetActive(false);
            startCanvas.transform.GetChild(0).gameObject.SetActive(true);
            TextMeshProUGUI textStart = startCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            textStart.text = "Move to be Ready";
            // textStart.fontSize *= 0.7f;
            // startCanvas.transform.GetChild(1).gameObject.SetActive(true);
        }

        public IEnumerator ShowGameStartUI()
        {
            startCanvas.transform.GetChild(1).gameObject.SetActive(false);
            startCanvas.transform.GetChild(0).gameObject.SetActive(true);
            TextMeshProUGUI textStart = startCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            // textStart.fontSize /= 0.7f;
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

        public IEnumerator ShowGameEndUI(float seconds, float angle, float strength, int coins)
        {
            startCanvas.SetActive(false);
            yield return new WaitForSeconds(1f);
            
            float time = 0;
            // rightAvatarCanvas.SetActive(true);
            leftAvatarCanvas.SetActive(true);
            
            // Vector3 rightNewPosition = rightAvatarCanvas.transform.position - rightAvatarCanvas.transform.right;
            // Vector3 leftNewPosition = leftAvatarCanvas.transform.position + leftAvatarCanvas.transform.right;
            
            while (time < 1f)
            {
                time += Time.deltaTime;
                // rightAvatarCanvas.transform.position = Vector3.Lerp(rightAvatarCanvas.transform.position, rightNewPosition, Time.deltaTime);
                // leftAvatarCanvas.transform.position = Vector3.Lerp(leftAvatarCanvas.transform.position, leftNewPosition, Time.deltaTime);
                leftForceProgressBar.UpdateFillAmount(time);
                // rightForceProgressBar.UpdateFillAmount(time);
                yield return null;
            }
           
            strength = (strength - 1) * 100;
            evaluationCanvas.SetActive(true);
            TimeSpan timeSpan = new TimeSpan((int)seconds / 60, (int)seconds % 60, (int)((seconds - (int)seconds) * 100));
            evaluationCanvas.transform.GetChild(0).GetChild(0).GetChild(0).Find("TimeText").GetComponent<TextMeshProUGUI>().text = timeSpan.ToString(@"hh\:mm\:ss");
            // evaluationCanvas.transform.GetChild(0).GetChild(0).GetChild(0).Find("AngleText")
            //     .GetComponent<TextMeshProUGUI>()
            //     .text = angle + "";
            evaluationCanvas.transform.GetChild(0).GetChild(0).GetChild(0).Find("PerformanceText").GetComponent<TextMeshProUGUI>()
                .text = strength > 0 ? "+ " + strength.ToString("0.0") + "%" : "+ 0%";
            // evaluationCanvas.transform.GetChild(0).GetChild(0).GetChild(0).Find("CoinsText").GetComponent<TextMeshProUGUI>().text =
            //     "+ " + coins;


            yield return new WaitForSeconds(15f);
            
            float move = gameManager.sensorManager.GetData(SensorPosition.LEFT);
            while (move < 5)
            {
                move += Mathf.Max(gameManager.sensorManager.GetData(SensorPosition.LEFT), gameManager.sensorManager.GetData(SensorPosition.RIGHT));
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