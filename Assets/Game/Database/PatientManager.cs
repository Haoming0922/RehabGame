using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Realms;
using RehabDB;
using static Realms.ThreadSafeReference;

public class PatientManager
{
    private Realm realm;

    public PatientManager()
    {
        realm = Realm.GetInstance();
    }

    public void AddPerformance(ObjectId patientID, string game, float left, float right)
    {
        var perf = new Performance()
        {
            gameType = game,
            leftInputPerformance = left,
            rightInputPerformance = right
        };
        
        var patient = realm.Find<Patient>(patientID);
        if (patient != null)
        {
            realm.Write(() =>
            {
                patient.performance.Add(perf);
            });
        }
    }
    
    
    
}
