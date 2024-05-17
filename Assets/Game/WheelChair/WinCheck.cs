using System;
using System.Collections;
using System.Collections.Generic;
using Game.Wheelchair;
using UnityEngine;

public class WinCheck : MonoBehaviour
{
    public GameManager2D gameManager2D;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            gameManager2D.gameEndEvent?.Invoke();
        }
    }
    
}
