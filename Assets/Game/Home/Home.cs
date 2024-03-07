using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Home : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JumpJump()
    {
        SceneManager.LoadScene("JumpJump");
    }
    
    public void Wheelchair()
    {
        SceneManager.LoadScene("WheelChair");
    }
}
