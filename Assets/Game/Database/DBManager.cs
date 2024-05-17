using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using Realms;
using Realms.Sync;
using UnityEngine;
using Game.Util;
using Unity.VisualScripting;
using MongoClient = Realms.Sync.MongoClient;

namespace RehabDB
{
    public class DBManager : Singleton<DBManager>
    {
        MongoClient client;
        MongoClient.Database database;
        private MongoClient.Collection<Patient> patientCollection;
        public Patient[] patients;
        public Patient currentPatient;

        public Action OnLoadFinish;
        public Action OnSwitchUser;

        void Start()
        {
            StartCoroutine(LoadDB());
        }

        IEnumerator LoadDB()
        {
            var myRealmAppId = "rehybunity-uklpqky";
            var app = App.Create(myRealmAppId);

            //connect realm instance
            var userTask = app.LogInAsync(Credentials.EmailPassword("s220056@dtu.dk", "sujuCY13"));
            yield return new WaitUntil(() => userTask.IsCompleted);

            //connect to mongodb atlas
            var user = userTask.Result;
            client = user.GetMongoClient("mongodb-atlas");
            database = client.GetDatabase("HDT_exercise");
            patientCollection = database.GetCollection<Patient>("patientProfile");

            yield return StartCoroutine(LoadCollection());
            
            OnLoadFinish?.Invoke();
        }

        public IEnumerator LoadData()
        {
            if(patientCollection != null)
            {
                StartCoroutine(LoadCollection());
                if (currentPatient != null)
                {
                    yield return StartCoroutine(LoadCurrentPatient(currentPatient.Name));
                    OnSwitchUser?.Invoke();
                }
            }
        }

        public IEnumerator LoadCurrentPatient(string name)
        {
            Debug.Log(name);

            if (patients != null)
            {
                foreach (var patient in patients)
                {
                    if (patient.Name == name)
                    {
                        currentPatient = patient;
                        break;
                    }
                }
            }
            else
            {
                var loadTask = patientCollection.FindOneAsync(
                    new { Name = name},
                    null,
                    null
                );
                yield return new WaitUntil(() => loadTask.IsCompleted);
            
                currentPatient = loadTask.Result;
                Debug.Log(currentPatient.Name);
            }
            
            OnSwitchUser?.Invoke();
        }
        

        private IEnumerator LoadCollection()
        {
            var loadTask = patientCollection.FindAsync(
                null, // new { Email = "andersen@gmail.com"}
                null,
                null
            );
            yield return new WaitUntil(() => loadTask.IsCompleted);
            
            Debug.Log(loadTask.Result.Length);
            
            patients = loadTask.Result;
            
            // OnLoadFinish?.Invoke();
            
            // try
            // {
            //     patients = await collection.FindAsync();
            //     UnityMainThreadDispatcher.Instance().Enqueue(() => {
            //         Debug.Log("Patients loaded");
            //         OnLoadFinish?.Invoke();
            //     });
            // }
            // catch (Exception ex)
            // {
            //     UnityMainThreadDispatcher.Instance().Enqueue(() => {
            //         Debug.LogError($"Failed to load patients: {ex.Message}");
            //     });
            // }
            
        }
        
        public async void SaveCurrentPatient()
        {
            if (currentPatient == null) return;
            var filter = new { Name = currentPatient.Name };
            await patientCollection.FindOneAndReplaceAsync(filter, currentPatient);
        }
        
        public async void SavePerformance(string game, float leftvalue, float rightvalue)
        {
            if(currentPatient == null) return;
            
            var patient = await patientCollection.FindOneAsync(
                new { Name = currentPatient.Name },
                null,
                null
            );
            if (patient != null)
            {
                Performance findPerformance = patient.FindGamePerformance(game);
                if (findPerformance == null)
                {
                    var newPerformance = new Performance();
                    newPerformance.gameType = game;
                    newPerformance.leftInputPerformance = leftvalue;
                    newPerformance.rightInputPerformance = rightvalue;
                    patient.performance.Add(newPerformance);
                }
                else
                {
                    findPerformance.leftInputPerformance = (findPerformance.leftInputPerformance + leftvalue) / 2f;
                    findPerformance.rightInputPerformance = (findPerformance.rightInputPerformance + rightvalue) / 2f;
                }
            }
            
            // var updateString = ConvertDicToString(patient.performance);
            // // update not permitted
            // var update = new BsonDocument("$set", new BsonDocument("performance", updateString));
            // // await _collection.UpdateOneAsync(filter, update);
            
            var filter = new { Name = currentPatient.Name  };
            await patientCollection.FindOneAndReplaceAsync(filter, patient);
        }
    }
}