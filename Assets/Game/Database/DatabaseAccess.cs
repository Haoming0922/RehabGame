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
using MongoClient = Realms.Sync.MongoClient;

namespace RehabDB
{
    public class DatabaseAccess : MonoBehaviour
    {
        MongoClient client;
        MongoClient.Database database;
        MongoClient.Collection<Patient> _collection;

        void Start()
        {
            StartCoroutine(StartAsync());
        }

        IEnumerator StartAsync()
        {
            //realm Credentials
            var myRealmAppId = "rehybunity-uklpqky";

            //create and config realm
            var app = App.Create(myRealmAppId);
            //var appConfig = new AppConfiguration(myRealmAppId)
            //{
            //    DefaultRequestTimeout = TimeSpan.FromMilliseconds(1500)
            //};

            //app = App.Create(appConfig);
            
            //connect realm instance
            var userTask = app.LogInAsync(Credentials.EmailPassword("s220056@dtu.dk", "sujuCY13"));
            yield return new WaitUntil(() => userTask.IsCompleted);

            //connect to mongodb atlas
            var user = userTask.Result;
            client = user.GetMongoClient("mongodb-atlas");
            database = client.GetDatabase("HDT_exercise");
            _collection = database.GetCollection<Patient>("patientProfile");

            //find patient
            // Task<Patient> patientTask = GetPatientDataAsync("patient_1@test.com", "123321");
            Task<Patient> patientTask = GetPatientDataAsync("sfsdfsdfsdf", "123456");
            yield return new WaitUntil(() => patientTask.IsCompleted);

            var patient = patientTask.Result;
            Debug.Log(patient.Avatar);

            // PatientManager pm = new PatientManager();
            // pm.AddPerformance(patient.Id, "Jump Jump", 10f, 12f);
            SavePerformance("sfsdfsdfsdf", "Jump Jump", 36f, 37f);

        }

        //read data by email
        public async Task<Patient> GetPatientDataAsync(string username, string password)
        {
            //linq is used for quering
            var patient = await _collection.FindOneAsync(
                new { Email = username},
                null,
                null
            );

            if (patient != null)
            {
                Debug.Log("found");
                return patient;
            }
            else
            {
                Debug.Log("Not found");
            }

            return null;
        }

        //update performance
        public async void SavePerformance(string email, string game, float leftvalue, float rightvalue)
        {
            var patient = await _collection.FindOneAsync(
                new { Email = email },
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
                    findPerformance.leftInputPerformance = leftvalue;
                    findPerformance.rightInputPerformance = rightvalue;
                }
            }
            
            // var updateString = ConvertDicToString(patient.performance);
            // // update not permitted
            // var update = new BsonDocument("$set", new BsonDocument("performance", updateString));
            // // await _collection.UpdateOneAsync(filter, update);
            
            var filter = new { Email = email };
            await _collection.FindOneAndReplaceAsync(filter, patient);
        }

        public string ConvertDicToString(IDictionary<string, Performance> dic)
        {
            var json = JsonUtility.ToJson(dic);
            return json;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}