using System;
using System.Collections;
using System.Collections.Generic;
using Game.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using RehabDB;

public class Home : MonoBehaviour
{
    public AvatarLoader avatarLoader;
    public GameObject homeMenu;
    public GameObject gamesMenu;
    public GameObject avatarMenu;
    public GameObject userMenu;
    public GameObject cellPrefab; // Set this in the inspector, should be a UI Text or your custom cell prefab
    public GridLayoutGroup gridLayoutGroup;

    public TextMeshProUGUI switchUserText;
    
    public GameObject info;
    public GameObject games;

    public TextMeshProUGUI thumbsAll;
    public TextMeshProUGUI thumbsContact;
    public TextMeshProUGUI thumbsTherapist;

    public GameObject taskInfoJumpJump;
    public GameObject taskInfoWheelchair;
    public GameObject taskInfoWheelchairFootball;
    public GameObject taskInfoCycle;

    private bool disPlayProgressbarJumpJump;
    private bool disPlayProgressbarWheelchair;
    private bool disPlayProgressbarWheelchairFootball;
    private bool disPlayProgressbarCycle;
    
    
    
    // public TextMeshProUGUI loginResult;
    
    private void Start()
    {
        // DBManager.Instance.OnLoadFinish += PopulateTable;
        DBManager.Instance.OnLoadFinish += ShowText;
        DBManager.Instance.OnSwitchUser += DisplayUserInfo;

        // StartCoroutine(DBManager.Instance.LoadData());
        if (DBManager.Instance.currentPatient != null)
        {
            ShowText();
            DBManager.Instance.OnSwitchUser?.Invoke();
        }

        // StartCoroutine(ComeOn());
    }

    private void OnDestroy()
    {
        // DBManager.Instance.OnLoadFinish -= PopulateTable;
        DBManager.Instance.OnLoadFinish -= ShowText;
        DBManager.Instance.OnSwitchUser -= DisplayUserInfo;
    }

    
    private void Update()
    {
    }

    private IEnumerator ComeOn()
    {
        while (true)
        {
            avatarLoader.Speak("come_on", new Dictionary<string, string>(){ {"player", DBManager.Instance.currentPatient.Name.Split(" ")[0]} });
            yield return new WaitForSeconds(10f);
        }
    }

    

    public void GoHome()
    {
        SceneManager.LoadScene("Home");
    }
    
    
    public IEnumerator BeforeGameCoroutine(string scene)
    {
        gamesMenu.SetActive(false);
        avatarMenu.SetActive(true);
        
        // therapistMenu.SetActive(true);
        // avatarController = therapistMenu.GetComponent<ReHybAvatarController>();
        // avatarController.TherapistSpeak("greeting");
        // yield return new WaitForSeconds(10f);
        // therapistMenu.SetActive(false);
        
        userMenu.SetActive(true);
        yield return new WaitForSeconds(8f);
        avatarLoader.Speak("start_without_competition", new Dictionary<string, string>(){ {"player", DBManager.Instance.currentPatient.Name.Split(" ")[0]} });
        yield return new WaitForSeconds(18f);
        
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
    
    public void Wheelchair1D()
    {
        StartCoroutine(BeforeGameCoroutine("WheelChair1D"));
    }
    
    public void Wheelchair2D()
    {
        StartCoroutine(BeforeGameCoroutine("WheelChair2D"));
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

    public void ShowText()
    {
        switchUserText.text = "Please select a participant to enter the game page: ";
    }

    public void PopulateTable()
    {
        foreach (var patient in DBManager.Instance.patients)
        {
            GameObject newCell = Instantiate(cellPrefab, gridLayoutGroup.transform);
            newCell.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = patient.Name; // Make sure your prefab has a Text component
            newCell.transform.localScale = Vector3.one; 
            newCell.GetComponent<UserCell>().patient = patient;
            newCell.SetActive(true);
        }
        // cellPrefab.SetActive(false);
    }

    public void DisplayUserInfo()
    {
        if (DBManager.Instance.currentPatient == null) return;

        DisplayAvatar();
        DisplayThumbs();
        DisplayProgress();
    }
    
    private void DisplayAvatar()
    {
        avatarLoader.LoadAvatar(DBManager.Instance.currentPatient.UnityAvatar);
        info.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = DBManager.Instance.currentPatient.Name;
        avatarLoader.currentAvatar.SetActive(false);
        avatarLoader.currentAvatar.SetActive(true);
    }

    private void DisplayThumbs()
    {
        thumbsAll.text = DBManager.Instance.currentPatient.thumbs + DBManager.Instance.currentPatient.thumbs_caregivers + "";
        thumbsContact.text = DBManager.Instance.currentPatient.thumbs_caregivers + "";
        thumbsTherapist.text = DBManager.Instance.currentPatient.thumbs + "";
    }
    
    private void DisplayProgress()
    {
        Debug.Log("Display Progress: " + DBManager.Instance.currentPatient.Name);
        disPlayProgressbarJumpJump = false;
        disPlayProgressbarWheelchair = false;
        disPlayProgressbarWheelchairFootball = false;
        disPlayProgressbarCycle = false;
        
        foreach (var task in DBManager.Instance.currentPatient.Tasks)
        {
            Debug.Log("Game Type: "  + task.game.type);
            if(Helper.IsCurrentDateInRange(task.date)) continue;
            
            if (task.game.type == "Jump Jump")
            {
                taskInfoJumpJump.SetActive(true);
                if (task.status == "Awaiting start" || !disPlayProgressbarJumpJump)
                {
                    taskInfoJumpJump.transform.GetChild(0).gameObject.SetActive(false);
                    taskInfoJumpJump.transform.GetChild(1).gameObject.SetActive(false);
                    taskInfoJumpJump.transform.GetChild(2).gameObject.SetActive(true);
                    taskInfoJumpJump.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = task.date;
                }
                else if (task.status == "In Process")
                {
                    taskInfoJumpJump.transform.GetChild(0).gameObject.SetActive(true);
                    taskInfoJumpJump.transform.GetChild(1).gameObject.SetActive(false);
                    taskInfoJumpJump.transform.GetChild(2).gameObject.SetActive(false);
                    taskInfoJumpJump.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = task.date;
                    
                    taskInfoJumpJump.transform.GetChild(0).GetChild(0).GetComponent<Game.Util.ClassicProgressBar>().UpdateFillAmount(task.spentTime / task.totalTime);
                    
                    disPlayProgressbarJumpJump = true;
                }
                else if (task.status == "Done" || !disPlayProgressbarJumpJump)
                {
                    taskInfoJumpJump.transform.GetChild(0).gameObject.SetActive(false);
                    taskInfoJumpJump.transform.GetChild(1).gameObject.SetActive(true);
                    taskInfoJumpJump.transform.GetChild(2).gameObject.SetActive(false);
                    taskInfoJumpJump.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = task.date;
                }
            }
            else if (task.game.type == "WheelChair")
            {
                taskInfoWheelchair.SetActive(true);
                if (task.status == "Awaiting start" || !disPlayProgressbarWheelchair)
                {
                    taskInfoWheelchair.transform.GetChild(0).gameObject.SetActive(false);
                    taskInfoWheelchair.transform.GetChild(1).gameObject.SetActive(false);
                    taskInfoWheelchair.transform.GetChild(2).gameObject.SetActive(true);
                    taskInfoWheelchair.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = task.date;
                }
                else if (task.status == "In Process")
                {
                    taskInfoWheelchair.transform.GetChild(0).gameObject.SetActive(true);
                    taskInfoWheelchair.transform.GetChild(1).gameObject.SetActive(false);
                    taskInfoWheelchair.transform.GetChild(2).gameObject.SetActive(false);
                    taskInfoWheelchair.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = task.date;
                    
                    taskInfoWheelchair.transform.GetChild(0).GetChild(0).GetComponent<Game.Util.ClassicProgressBar>().UpdateFillAmount(task.spentTime / task.totalTime);
                    
                    disPlayProgressbarWheelchair = true;
                }
                else if (task.status == "Done" || !disPlayProgressbarWheelchair)
                {
                    taskInfoWheelchair.transform.GetChild(0).gameObject.SetActive(false);
                    taskInfoWheelchair.transform.GetChild(1).gameObject.SetActive(true);
                    taskInfoWheelchair.transform.GetChild(2).gameObject.SetActive(false);
                    taskInfoWheelchair.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = task.date;
                }
            }
            else if (task.game.type == "Wheelchair football")
            {
                taskInfoWheelchairFootball.SetActive(true);
                if (task.status == "Awaiting start" || !disPlayProgressbarWheelchairFootball)
                {
                    taskInfoWheelchairFootball.transform.GetChild(0).gameObject.SetActive(false);
                    taskInfoWheelchairFootball.transform.GetChild(1).gameObject.SetActive(false);
                    taskInfoWheelchairFootball.transform.GetChild(2).gameObject.SetActive(true);
                    taskInfoWheelchairFootball.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = task.date;
                }
                else if (task.status == "In Process")
                {
                    taskInfoWheelchairFootball.transform.GetChild(0).gameObject.SetActive(true);
                    taskInfoWheelchairFootball.transform.GetChild(1).gameObject.SetActive(false);
                    taskInfoWheelchairFootball.transform.GetChild(2).gameObject.SetActive(false);
                    taskInfoWheelchairFootball.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = task.date;
                    
                    taskInfoWheelchairFootball.transform.GetChild(0).GetChild(0).GetComponent<Game.Util.ClassicProgressBar>().UpdateFillAmount(task.spentTime / task.totalTime);
                    
                    disPlayProgressbarWheelchairFootball = true;
                }
                else if (task.status == "Done" || !disPlayProgressbarWheelchairFootball)
                {
                    taskInfoWheelchairFootball.transform.GetChild(0).gameObject.SetActive(false);
                    taskInfoWheelchairFootball.transform.GetChild(1).gameObject.SetActive(true);
                    taskInfoWheelchairFootball.transform.GetChild(2).gameObject.SetActive(false);
                    taskInfoWheelchairFootball.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = task.date;
                }
            }
            else if (task.game.type == "Cycle")
            {
                taskInfoCycle.SetActive(true);
                if (task.status == "Awaiting start" || !disPlayProgressbarCycle)
                {
                    taskInfoCycle.transform.GetChild(0).gameObject.SetActive(false);
                    taskInfoCycle.transform.GetChild(1).gameObject.SetActive(false);
                    taskInfoCycle.transform.GetChild(2).gameObject.SetActive(true);
                    taskInfoCycle.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = task.date;
                }
                else if (task.status == "In Process")
                {
                    taskInfoCycle.transform.GetChild(0).gameObject.SetActive(true);
                    taskInfoCycle.transform.GetChild(1).gameObject.SetActive(false);
                    taskInfoCycle.transform.GetChild(2).gameObject.SetActive(false);
                    taskInfoCycle.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = task.date;
                    
                    taskInfoCycle.transform.GetChild(0).GetChild(0).GetComponent<Game.Util.ClassicProgressBar>().UpdateFillAmount(task.spentTime / task.totalTime);
                    
                    disPlayProgressbarCycle = true;
                }
                else if (task.status == "Done" || !disPlayProgressbarCycle)
                {
                    taskInfoCycle.transform.GetChild(0).gameObject.SetActive(false);
                    taskInfoCycle.transform.GetChild(1).gameObject.SetActive(true);
                    taskInfoCycle.transform.GetChild(2).gameObject.SetActive(false);
                    taskInfoCycle.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = task.date;
                }
            }

        }
    }
    

    #region Archive

            
        // private IEnumerator PostLoginResult(string result)
        // {
        //     loginResult.text = "Login " + result;
        //     yield return new WaitForSeconds(2f); 
        //     loginResult.text = "";
        // }
        //
        //     
        //         public void ShowTask()
        //     {
        //         if (DBConnector.Instance.currentPatient == "") return;
        //         for (int i = 0; i < games.transform.childCount; i++)
        //         {
        //             games.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
        //         }
        //
        //         foreach (var gameTask in DBConnector.Instance.Patients[DBConnector.Instance.currentPatient].GameTasks)
        //         {
        //             if (gameTask.game.type == "Jump Jump")
        //             {
        //                 games.transform.Find("JumpJump").GetChild(1).gameObject.SetActive(true);
        //                 games.transform.Find("JumpJump").GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
        //                     "Task: " + gameTask.sets * gameTask.game.slots + " min";
        //             }
        //             else if (gameTask.game.type == "WheelChair")
        //             {
        //                 games.transform.Find("WheelChair").GetChild(1).gameObject.SetActive(true);
        //                 games.transform.Find("WheelChair").GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
        //                     "Task: " + gameTask.sets * gameTask.game.slots + " min";
        //             }
        //             else if (gameTask.game.type == "Cycle")
        //             {
        //                 games.transform.Find("Cycle").GetChild(1).gameObject.SetActive(true);
        //                 games.transform.Find("Cycle").GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
        //                     "Task: " + gameTask.sets * gameTask.game.slots + " min";
        //             }
        //         }
        //     }
        //     
        //     public void ShowInfo()
        //     {
        //         if (DBConnector.Instance.currentPatient == "") return;
        //         Debug.Log("Haoming: " + DBConnector.Instance.currentPatient);
        //         info.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
        //             DBConnector.Instance.Patients[DBConnector.Instance.currentPatient].Name;
        //     }
        //     
        //     public void Login()
        //     {
        //         DBConnector.Instance.CheckLogin(emailInput.text, pswInput.text, result =>
        //         {
        //             if (result)
        //             {
        //                 DBConnector.Instance.currentPatient = emailInput.text;
        //                 StartCoroutine(PostLoginResult("Success"));
        //                 loginMenu.SetActive(false);
        //                 homeMenu.SetActive(true);
        //                 ShowInfo();
        //                 ShowTask();
        //             }
        //             else
        //             {
        //                 StartCoroutine(PostLoginResult("Fail"));
        //                 pswInput.text = "";
        //             }
        //         });
        //     }
    

    #endregion
    
    
    
}
