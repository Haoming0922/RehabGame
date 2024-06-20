

namespace DataBase2
{

    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Networking;
    using Newtonsoft.Json;

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




            Task<Patient> patientTask = GetPatientByEmail("Natalie@test.com");
            yield return new WaitUntil(() => patientTask.IsCompleted);
            var patient = patientTask.Result;
            Debug.Log(patient.name);

            SavePatientPerformance(patient, "WheelChair", 30.0f, 30.0f);
            GameTask gameTask = patient.Tasks[3];
            Performance newPerformance = new Performance("Cycle", 4.0f, 4.0f, "05/24/2024", "05/24/2024", 10.0);
            SaveTaskPerformance(patient.patientID, gameTask, newPerformance);

        }

        public async Task<List<Patient>> GetAllPaitent()
        {
            string url = "http://localhost:8090/patient/allPatient";

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
            string url = "http://localhost:8090/patient/findPatient";
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
            string url = "http://localhost:8090/patient/UpdatePatientPerformance";
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

        public async void SaveTaskPerformance(string patientID, GameTask currentTask, Performance newPerformance)
        {
            string url = "http://localhost:8090/patient/tasks";
            //operate task
            currentTask.performance.Add(newPerformance);
            var requestData = new
            {
                patientID = patientID,
                taskinfo = currentTask
            };

            //update performance in tasks
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