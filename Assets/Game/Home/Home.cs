using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Home : MonoBehaviour
{
    public void GoHome()
    {
        SceneManager.LoadScene("Home");
    }
    
    public void BackToPair()
    {
        SceneManager.LoadScene("Home");
        GameObject.Find("HomeMenu").SetActive(false);
        GameObject.Find("PairMenu").SetActive(true);
    }
    
    public void JumpJump()
    {
        SceneManager.LoadScene("Jump");
    }
    
    public void Wheelchair()
    {
        SceneManager.LoadScene("WheelChair");
    }
    
    public void Cycle()
    {
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
