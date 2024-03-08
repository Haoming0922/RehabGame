using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Home : MonoBehaviour
{
    public void JumpJump()
    {
        SceneManager.LoadScene("Jump");
    }
    
    public void Wheelchair()
    {
        SceneManager.LoadScene("WheelChair");
    }
}
