using System;
using System.Collections;
using System.Collections.Generic;
using Inworld;
using Inworld.Interactions;
using Inworld.Assets;
using Inworld.Packet;
using Unity.VisualScripting;
using UnityEngine;

public class ReHybAvatarController : MonoBehaviour
{
    [SerializeField] InworldCharacter m_CurrentCharacter;
    
    private void Update()
    {
    }

    [ContextMenu("UserSpeak")]
    public void UserSpeak()
    {
        m_CurrentCharacter.SendTrigger("greeting",true);
        m_CurrentCharacter.SendTrigger("greeting",true);
    }
    
    [ContextMenu("TherapistSpeak")]
    public void TherapistSpeak()
    {
        m_CurrentCharacter.SendTrigger("greeting",true);
        m_CurrentCharacter.SendTrigger("greeting",true);
    }
    
}
