//
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using MongoDB.Driver;
// using MongoDB.Bson;
// using Game.Util;
// using MongoDB.Bson.Serialization.Conventions;
// using MongoDB.Driver.Linq;
// using MongoDB.Bson.Serialization;
// using System.Threading.Tasks;
// using System;
//
//
// public class DBConnector : Singleton<DBConnector>
// {
//     private MongoClient client;
//     private IMongoDatabase database;
//     IMongoCollection<Patient> patientCollection;
//     IMongoCollection<LoginInfo> loginInfoCollection;
//     
//     public Dictionary<string,Patient> Patients = new Dictionary<string, Patient>(); // Your data items to display
//     public Dictionary<string,string> LoginInfos = new Dictionary<string, string>(); // Your data items to display
//
//     public string currentPatient = "";
//     
//     void Start()
//     {
//         StartCoroutine(StartAsync());
//     }
//     
//     IEnumerator StartAsync()
//     {
//         client = new MongoClient("mongodb+srv://S22Thesis1:123321@cluster0.k7tmgae.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0");
//
//         //automapping
//         var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
//         ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);
//
//         //connection
//         database = client.GetDatabase("HDT_exercise");
//         patientCollection = database.GetCollection<Patient>("patientProfile");
//         // loginInfoCollection = database.GetCollection<LoginInfo>("signUpUsers");
//         
//         Debug.Log(patientCollection.ToString());
//         
//         StartCoroutine(LoadPatientData(patientCollection));
//         // LoadLoginData(loginInfoCollection);
//         
//         yield return null;
//         // home.PopulateTable();
//     }
//     
//     
//     IEnumerator LoadPatientData(IMongoCollection<Patient> patientCollection)
//     {
//         var filter = Builders<Patient>.Filter.Empty;
//         var findTask = patientCollection.Find(filter).ToList();
//         
//         Debug.Log(findTask);
//         Debug.Log(findTask[0]);
//         
//         // while (!findTask.IsCompleted)
//         //     yield return null; // Wait until the async operation completes
//
//         // if (findTask.IsFaulted)
//         // {
//         //     Debug.LogError("Haoming Failed to load patients: " + findTask.Exception.ToString());
//         // }
//         // else
//         // {
//         //     foreach (var doc in findTask.Result)
//         //     {
//         //         Debug.Log("Haoming: " + doc.Email);
//         //         Patients.TryAdd(doc.Email, doc); // Assuming Email is unique and exists
//         //     }
//         // }
//
//         yield return null;
//
//     }
//
//     
//     void LoadLoginData(IMongoCollection<LoginInfo> loginInfoCollection)
//     {
//         var documents = loginInfoCollection.Find(Builders<LoginInfo>.Filter.Empty).ToList();
//         foreach (var doc in documents)
//         {
//             LoginInfos.Add(doc.Email, doc.Password);
//         }
//     }
//     
//     
//     public void CheckLogin(string email, string psw, Action<bool> onCompleted)
//     {
//         bool match =  LoginInfos.ContainsKey(email) && LoginInfos[email] == psw;
//         Debug.Log("HaomingCheck: " + LoginInfos.ContainsKey(email) + " " + LoginInfos[email]);
//         onCompleted?.Invoke(match);
//     }
//     
//     //read data by email
//     public async Task<Patient> GetPatientDataAsync(string email)
//     {
//         return await patientCollection.AsQueryable()
//             .Where(p =>  p.Email == email)
//             .FirstOrDefaultAsync();
//     }
//     
//     
//     //read performance by game
//     public object GetHistoricPerformance(Patient p, MiniGame game)
//     {
//         if (!p.performance.ContainsKey(game))
//         {
//             Performance performance = new Performance();
//             if(game == MiniGame.Wheelchair)
//             {
//                 performance.leftInputPerformance = 45.0f;
//                 performance.rightInputPerformance = 45.0f;
//
//             }else if (game == MiniGame.Dumbbell)
//             {
//                 performance.leftInputPerformance = 45.0f;
//                 performance.rightInputPerformance = 45.0f;
//
//             }else if (game == MiniGame.Cycle)
//             {
//                 performance.leftInputPerformance = 45.0f;
//                 performance.rightInputPerformance = 45.0f;
//             }
//
//             p.performance.Add(game, performance);
//         }
//        
//         return p.performance[game];
//     }
//
//     //update performance
//     public async void SavePerformance(string email, MiniGame game)
//     {
//         var query = await patientCollection.AsQueryable().Where(p => p.Email == email).FirstOrDefaultAsync();
//         if (query != null)
//         {
//             float value = 2.2f;
//             var update = Builders<Patient>.Update.Set(p => p.performance[game].leftInputPerformance, value)
//                 .Set(p => p.performance[game].rightInputPerformance, value);
//             await patientCollection.UpdateOneAsync(p => p.Email == email, update);
//         }
//     }
//
//     //update patient progress data
//     public async void SavePatientData(string email,string progress,float time)
//     {
//         //reset data
//         var query = await patientCollection.AsQueryable().Where(p=>p.Email==email).FirstOrDefaultAsync();
//         if (query != null)
//         {
//             //reset object here
//             var update = Builders<Patient>.Update.Set(p => p.progress, 1.5);
//             //.Set(p => p.Property2, newValue2);
//
//             await patientCollection.UpdateOneAsync(p => p.Email == email, update);
//         }
//     }
// }
