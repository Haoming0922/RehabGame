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
    public InworldCharacter m_CurrentCharacter;
    
    private void Update()
    {
    }
    [ContextMenu("UserSpeak")]
    public void UserSpeak(string goal)
    {
        m_CurrentCharacter.EnableGoal(goal);
        m_CurrentCharacter.SendTrigger(goal,true);
        m_CurrentCharacter.SendTrigger(goal,true);
    }
    

    [ContextMenu("TherapistSpeak")]
    public void TherapistSpeak(string goal)
    {
        m_CurrentCharacter.EnableGoal(goal);
        m_CurrentCharacter.SendTrigger(goal,true);
        m_CurrentCharacter.SendTrigger(goal,true);
    }
    
}
