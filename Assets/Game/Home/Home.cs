using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Home : MonoBehaviour
{
    private ReHybAvatarController avatarController;
    public GameObject homeMenu;
    public GameObject therapistMenu;
    public GameObject userMenu;

    
    public void GoHome()
    {
        SceneManager.LoadScene("Home");
    }

    public IEnumerator BeforeGame()
    {
        yield return StartCoroutine(BeforeGameCoroutine());
    }
    public IEnumerator BeforeGameCoroutine()
    {
        homeMenu.SetActive(false);
        therapistMenu.SetActive(true);
        avatarController = therapistMenu.GetComponent<ReHybAvatarController>();
        avatarController.TherapistSpeak();
        yield return new WaitForSeconds(10f);
        
        therapistMenu.SetActive(false);
        userMenu.SetActive(true);
        avatarController = userMenu.GetComponent<ReHybAvatarController>();
        avatarController.UserSpeak();
        yield return new WaitForSeconds(10f);
        
        userMenu.SetActive(false);
    }
    
    
    public void BackToPair()
    {
        SceneManager.LoadScene("Home");
        GameObject.Find("Home").SetActive(false);
        GameObject.Find("Pair").SetActive(true);
    }
    
    public void JumpJump()
    {
        StartCoroutine(BeforeGame());
        SceneManager.LoadScene("Jump");
    }
    
    public void Wheelchair()
    {
        StartCoroutine(BeforeGame());
        SceneManager.LoadScene("WheelChair");
    }
    
    public void Cycle()
    {
        StartCoroutine(BeforeGame());
        SceneManager.LoadScene("Cycle");
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
    
}
