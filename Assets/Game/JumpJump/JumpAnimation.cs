using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAnimation : MonoBehaviour
{
    public GameObject avatar;
    private Animator animator;
    
    private void Start()
    {
        animator = avatar.GetComponent<Animator>();
    }

    public void Idle()
    {
        animator.Play("F_Dances_001");
    }
    
    public void Win()
    {
        animator.Play("F_Dances_004");
    }
    
}
