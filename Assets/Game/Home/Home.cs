using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Home : MonoBehaviour
{
    private ReHybAvatarController avatarController;
    public GameObject homeMenu;
    public GameObject gamesMenu;
    public GameObject loginMenu;
    public GameObject therapistMenu;
    public GameObject userMenu;
    public GameObject cellPrefab; // Set this in the inspector, should be a UI Text or your custom cell prefab
    public GridLayoutGroup gridLayoutGroup;

    public TMP_InputField emailInput;
    public TMP_InputField pswInput;
    public TextMeshProUGUI loginResult;

    public GameObject info;
    public GameObject games;
    


    private void Update()
    {

    }

    
    public void ShowTask()
    {
        if (DBConnector.Instance.currentPatient == "") return;
        for (int i = 0; i < games.transform.childCount; i++)
        {
            games.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
        }

        foreach (var gameTask in DBConnector.Instance.Patients[DBConnector.Instance.currentPatient].GameTasks)
        {
            if (gameTask.game.type == "Jump Jump")
            {
                games.transform.Find("JumpJump").GetChild(1).gameObject.SetActive(true);
                games.transform.Find("JumpJump").GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    "Task: " + gameTask.sets * gameTask.game.slots + " min";
            }
            else if (gameTask.game.type == "WheelChair")
            {
                games.transform.Find("WheelChair").GetChild(1).gameObject.SetActive(true);
                games.transform.Find("WheelChair").GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    "Task: " + gameTask.sets * gameTask.game.slots + " min";
            }
            else if (gameTask.game.type == "Cycle")
            {
                games.transform.Find("Cycle").GetChild(1).gameObject.SetActive(true);
                games.transform.Find("Cycle").GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    "Task: " + gameTask.sets * gameTask.game.slots + " min";
            }
        }
    }
    
    public void ShowInfo()
    {
        if (DBConnector.Instance.currentPatient == "") return;
        Debug.Log("Haoming: " + DBConnector.Instance.currentPatient);
        info.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
            DBConnector.Instance.Patients[DBConnector.Instance.currentPatient].Name;
    }
    
    public void Login()
    {
        DBConnector.Instance.CheckLogin(emailInput.text, pswInput.text, result =>
        {
            if (result)
            {
                DBConnector.Instance.currentPatient = emailInput.text;
                StartCoroutine(PostLoginResult("Success"));
                loginMenu.SetActive(false);
                homeMenu.SetActive(true);
                ShowInfo();
                ShowTask();
            }
            else
            {
                StartCoroutine(PostLoginResult("Fail"));
                pswInput.text = "";
            }
        });
    }
    
    private IEnumerator PostLoginResult(string result)
    {
        loginResult.text = "Login " + result;
        yield return new WaitForSeconds(2f); 
        loginResult.text = "";
    }
    

    public void GoHome()
    {
        SceneManager.LoadScene("Home");
    }
    
    
    public IEnumerator BeforeGameCoroutine(string scene)
    {
        gamesMenu.SetActive(false);
        
        therapistMenu.SetActive(true);
        avatarController = therapistMenu.GetComponent<ReHybAvatarController>();
        avatarController.TherapistSpeak("greeting");
        yield return new WaitForSeconds(10f);
        therapistMenu.SetActive(false);
        
        userMenu.SetActive(true);
        avatarController = userMenu.GetComponent<ReHybAvatarController>();
        avatarController.UserSpeak("greeting");
        yield return new WaitForSeconds(10f);
        
        userMenu.SetActive(false);
        SceneManager.LoadScene(scene);
    }
    
    
    public void BackToPair()
    {
        SceneManager.LoadScene("Home");
        GameObject.Find("Home").SetActive(false);
        GameObject.Find("Pair").SetActive(true);
    }
    
    public void JumpJump()
    {
        StartCoroutine(BeforeGameCoroutine("Jump"));
    }
    
    public void Wheelchair()
    {
        StartCoroutine(BeforeGameCoroutine("WheelChair"));
    }
    
    public void Cycle()
    {
        
        StartCoroutine(BeforeGameCoroutine("Cycle"));
    }
    
    public void DumbbellPair()
    {
        SceneManager.LoadScene("DumbbellPair");
    }
    
    public void WheelchairPair()
    {
        SceneManager.LoadScene("WheelchairPair");
    }
    
    public void CyclePair()
    {
        SceneManager.LoadScene("CyclePair");
    }
    
    public void PopulateTable()
    {
        foreach (var patient in DBConnector.Instance.Patients)
        {
            GameObject newCell = Instantiate(cellPrefab, gridLayoutGroup.transform);
            newCell.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = patient.Value.Email; // Make sure your prefab has a Text component
        }
        // cellPrefab.SetActive(false);
    }
    
    
    
}
