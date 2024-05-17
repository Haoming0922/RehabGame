using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using ReadyPlayerMe.Core;
using UnityEngine;
using Inworld;
using Game.Util;
using Inworld.Sample;
using JetBrains.Annotations;
using Realms;
using UnityEngine.SceneManagement;

namespace RehabDB
{
    public class AvatarLoader : MonoBehaviour
    {
        [SerializeField] private GameObject maleAvatar;
        [SerializeField] private GameObject femaleAvatar;
        [SerializeField] private GameObject maleAvatarArmature;
        [SerializeField] private GameObject femaleAvatarArmature;
        
        [SerializeField] private GameObject playerArmature;
        
        public GameObject currentAvatar;
        public GameObject currentAvatarArmature;
        private string currentAvatatURL = "";
        
        public InworldCharacter m_CurrentCharacter;
        public InworldClient client;
        
        // [SerializeField] private Transform userPageAvatarParent;
        
        private AvatarObjectLoader avatarObjectLoader;
        [SerializeField][Tooltip("Preview avatar to display until avatar loads. Will be destroyed after new avatar is loaded")]

        public event Action OnLoadComplete;
        
        private void Start()
        {
            avatarObjectLoader = new AvatarObjectLoader();
            avatarObjectLoader.OnCompleted += OnLoadCompleted;
            avatarObjectLoader.OnFailed += OnLoadFailed;
            m_CurrentCharacter = currentAvatar.GetComponent<InworldCharacter>();
            
        }
        
        private void OnDestroy()
        {
            avatarObjectLoader.OnCompleted -= OnLoadCompleted;
            avatarObjectLoader.OnFailed -= OnLoadFailed;
        }
        

        private void OnLoadFailed(object sender, FailureEventArgs args)
        {
            currentAvatar.SetActive(true);
            OnLoadComplete?.Invoke();
        }

        private void OnLoadCompleted(object sender, CompletionEventArgs args)
        {
            // if (previewAvatar != null)
            // {
            //     previewAvatar.name = "ArmaturePreview";
            //     previewAvatar.SetActive(false);
            // }
            // if (loadAvatar != null)
            // {
            //     Destroy(loadAvatar);
            // }

            SetupAvatarInWorld(args.Avatar);
            // SetupAvatarRDM(args.Avatar);
            OnLoadComplete?.Invoke();
        }

        public void SetupAvatarGame()
        {
            Debug.Log("Haoming " + currentAvatarArmature.transform.position);
            GameObject newArmature = Instantiate(currentAvatarArmature, playerArmature.transform.parent);
            Debug.Log("Haoming " + newArmature.transform.position);
            Destroy(playerArmature);
            currentAvatarArmature = newArmature;
            Debug.Log("Haoming " + newArmature.transform.rotation);
            
            playerArmature.transform.parent.gameObject.SetActive(false);
            playerArmature.transform.parent.gameObject.SetActive(true);
        }

        private void SetupAvatarInWorld(GameObject targetAvatar)
        {
            GameObject armature = targetAvatar.transform.GetChild(0).gameObject;
            targetAvatar.transform.GetChild(1).SetParent(armature.transform);
            
            if (DBManager.Instance.currentPatient.Sexual == "male")
            {
                femaleAvatar.SetActive(false);
                currentAvatar.SetActive(false);
                if(maleAvatarArmature != null) Destroy(maleAvatarArmature);
                armature.transform.SetParent(maleAvatar.transform);
                maleAvatarArmature = armature;
                currentAvatar = maleAvatar;
                currentAvatarArmature = maleAvatarArmature;
            }
            else
            {
                maleAvatar.SetActive(false);
                currentAvatar.SetActive(false);
                if(femaleAvatarArmature != null) Destroy(femaleAvatarArmature);
                armature.transform.SetParent(femaleAvatar.transform);
                femaleAvatarArmature = armature;
                currentAvatar = femaleAvatar;
                currentAvatarArmature = femaleAvatarArmature;
            }
            
            targetAvatar.SetActive(false);
            
            armature.transform.localPosition = Vector3.zero;
            armature.transform.localRotation = Quaternion.identity;
            armature.transform.localScale = Vector3.one;

            m_CurrentCharacter = currentAvatar.GetComponent<InworldCharacter>();
            
            currentAvatar.SetActive(false);
            currentAvatar.SetActive(true);
        }
        
        // private void SetupAvatarRDM(GameObject targetAvatar)
        // {
        //     Transform armature = targetAvatar.transform.GetChild(0);
        //
        //     while (targetAvatar.transform.childCount > 1)
        //     {
        //         targetAvatar.transform.GetChild(1).SetParent(armature);
        //     }
        //     
        //     // armature.SetParent(avatarParent);
        //     
        //     // armature.parent = null;
        //     // TransformOperator.ReparentAllChildren(targetAvatar, armature.gameObject);
        //     // armature.SetParent(avatarParent);
        //     Destroy(targetAvatar);
        //     
        //     loadAvatar = armature.gameObject;
        //     loadAvatar.transform.localPosition = Vector3.zero;
        //     loadAvatar.transform.localRotation = Quaternion.identity;
        //     loadAvatar.transform.localScale = Vector3.one;
        //     
        //     loadAvatar.transform.parent.gameObject.SetActive(false);
        //     loadAvatar.transform.parent.gameObject.SetActive(true);
        //     
        // }

        // public void SetUpUserPageAvatar()
        // {
        //     if(loadAvatar == null) return;
        //     userPageAvatarParent.GetChild(1).gameObject.name = "ArmaturePreview";
        //     userPageAvatarParent.GetChild(1).gameObject.SetActive(false);
        //     loadAvatar.transform.SetParent(userPageAvatarParent);
        //     loadAvatar.transform.parent.gameObject.SetActive(false);
        //     loadAvatar.transform.parent.gameObject.SetActive(true);
        // }

        public void LoadAvatar(string url)
        {
            if(url == "") return;
            
            if (currentAvatatURL == url && currentAvatar != null)
            {
                // currentAvatar.transform.position = homePosition;
                // currentAvatar.transform.rotation = homeRotaion;
                return;
            }
            
            // if (DBManager.Instance.currentPatient.Avatar != null)
            // {
            //     currentAvatar.SetActive(false);
            //     // currentAvatar.name = "ArmaturePreview";
            // }

            currentAvatar.SetActive(false);
            avatarObjectLoader.LoadAvatar(url.Trim(' '));
            currentAvatatURL = url;
        }

        
        [ContextMenu("UserSpeak")]
        public void Speak(string goal, Dictionary<string, string> parameters)
        {
            // Debug.Log("Speak: " + goal);
            m_CurrentCharacter.SendTrigger(goal, false, parameters);
            m_CurrentCharacter.SendTrigger(goal, false, parameters);
        }
        
        
    }
}
