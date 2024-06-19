using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;



namespace RehabDB2
{
    public class DatabaseAccess : MonoBehaviour
    {

        void Start()
        {
            StartCoroutine(StartAsync());
        }

        IEnumerator StartAsync()
        {

            //Task<List<Patient>> patientListTask = GetAllPaitent();
            //yield return new WaitUntil(() => patientListTask.IsCompleted);

            //var patientList = patientListTask.Result;
            //foreach(Patient p in patientList)
            //{
            //    Debug.Log(p.name);
            //}

            Task<Patient> patientTask = GetPatientByEmail("");
            yield return new WaitUntil(() => patientTask.IsCompleted);
            var patient = patientTask.Result;
            Debug.Log(patient.name);

            SavePatientPerformance(patient, "WheelChair", 30.0f, 30.0f);

        }

        public async Task<List<Patient>> GetAllPaitent()
        {
            string url = "";

            UnityWebRequest request = await GetRequest(url);
            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
                return null;
            }
            else
            {
                // process data
                string jsonResponse = request.downloadHandler.text;
                List<Patient> responseData = JsonConvert.DeserializeObject<List<Patient>>(jsonResponse);
                return responseData;
            }
        }


        public async Task<Patient> GetPatientByEmail(string email)
        {
            string url = "";
            UnityWebRequest request = await PostRequest(url, email);

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
                return null;
            }
            else
            {
                // process data
                string responseJson = request.downloadHandler.text;
                Patient responseData = JsonConvert.DeserializeObject<Patient>(responseJson);
                return responseData;
            }
        }



        //update performance
        public async void SavePatientPerformance(Patient p, string gameType, float leftvalue, float rightvalue)
        {
            string url = "";
            Performance performance = new Performance();
            performance.gameType = gameType;
            performance.left = leftvalue;
            performance.right = rightvalue;
            //update performance in patient
            var requestData = new
            {
                patientID = p.patientID,
                performance = performance
            };

            UnityWebRequest request = await PostRequest(url, requestData);
            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                // process data
                string responseJson = request.downloadHandler.text;
                List<GameTask> responseData = JsonConvert.DeserializeObject<List<GameTask>>(responseJson);

                if (responseData != null)
                {
                    Debug.Log("update current gametask");
                }
            }


        }

        public async void SaveTaskPerformance(Patient p, string taskId, float leftvalue, float rightvalue)
        {
            string url = "";
            List<GameTask> patientTaskList = p.Tasks;
            GameTask updatedTask = new GameTask();
            foreach (GameTask gameTask in patientTaskList)
            {
                if (gameTask._id == taskId)
                {
                    //operate task

                    updatedTask = gameTask;
                    break;
                }
            }

            //update performance in tasks
            UnityWebRequest request = await PostRequest(url, updatedTask);
            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                // process data
                string responseJson = request.downloadHandler.text;
                List<GameTask> responseData = JsonConvert.DeserializeObject<List<GameTask>>(responseJson);

                if (responseData != null)
                {
                    Debug.Log("update current gametask");
                }
            }


        }



        //encapsulate the get
        public async Task<UnityWebRequest> GetRequest(string url)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            //send request
            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            return request;
        }

        //encapsulate the post
        public async Task<UnityWebRequest> PostRequest<T>(string url, T data)
        {

            string jsonData = JsonConvert.SerializeObject(data);

            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            //send request
            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            return request;
        }


    }
}
