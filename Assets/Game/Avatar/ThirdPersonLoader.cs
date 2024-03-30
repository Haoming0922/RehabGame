using System;
using ReadyPlayerMe.Core;
using UnityEngine;
using ReadyPlayerMe.Samples.QuickStart;
using Unity.VisualScripting;

namespace Game.Avatar
{
    public class ThirdPersonLoader : MonoBehaviour
    {
        [SerializeField] private Transform previewTransform ;
        
        [SerializeField][Tooltip("RPM avatar URL or shortcode to load")] 
        private string avatarUrl;   
        private GameObject avatar;
        private AvatarObjectLoader avatarObjectLoader;
        [SerializeField][Tooltip("Animator to use on loaded avatar")] 
        private RuntimeAnimatorController animatorController;
        [SerializeField][Tooltip("If true it will try to load avatar from avatarUrl on start")] 
        private bool loadOnStart = true;
        [SerializeField][Tooltip("Preview avatar to display until avatar loads. Will be destroyed after new avatar is loaded")]
        private GameObject previewAvatar;

        [SerializeField] private UnityEngine.Avatar avatarToSet;

        public event Action OnLoadComplete;
        
        private void Start()
        {
            avatarObjectLoader = new AvatarObjectLoader();
            avatarObjectLoader.OnCompleted += OnLoadCompleted;
            avatarObjectLoader.OnFailed += OnLoadFailed;
            
            if (previewAvatar != null)
            {
                // SetupAvatar(previewAvatar);
            }
            if (loadOnStart)
            {
                LoadAvatar(avatarUrl);
            }
        }

        private void OnLoadFailed(object sender, FailureEventArgs args)
        {
            OnLoadComplete?.Invoke();
        }

        private void OnLoadCompleted(object sender, CompletionEventArgs args)
        {
            if (previewAvatar != null)
            {
                // Destroy(previewAvatar);
                previewAvatar.SetActive(false);
            }
            SetupAvatar(args.Avatar);
            OnLoadComplete?.Invoke();
        }

        private void SetupAvatar(GameObject targetAvatar)
        {
            if (avatar != null)
            {
                Destroy(avatar);
            }
            
            avatar = targetAvatar;
            // Re-parent and reset transforms
            avatar.transform.parent = transform.parent;
            avatar.transform.position = previewTransform.position;
            avatar.transform.localRotation = Quaternion.Euler(0, 180, 0);
            avatar.transform.localScale = new Vector3(60, 60, 60);
            
            SetChildTransform(avatar.transform);
            
            Destroy(previewAvatar);           
            
            Animator animator = avatar.GetComponent<Animator>();
            if (animator != null)
            {
                animator.runtimeAnimatorController = animatorController;
                animator.avatar = avatarToSet;
                animator.applyRootMotion = false;
            }
            
        }

        public void LoadAvatar(string url)
        {
            //remove any leading or trailing spaces
            avatarUrl = url.Trim(' ');
            avatarObjectLoader.LoadAvatar(avatarUrl);
        }

        private void SetChildTransform(Transform avatar)
        {
            Transform leftShoulder = avatar.Find("Armature").Find("Hips").Find("Spine").Find("Spine1").Find("Spine2").Find("LeftShoulder");
            Transform leftHand = leftShoulder.GetChild(0).GetChild(0).GetChild(0);
            Transform rightShoulder = avatar.Find("Armature").Find("Hips").Find("Spine").Find("Spine1").Find("Spine2").Find("RightShoulder");
            Transform rightHand = rightShoulder.GetChild(0).GetChild(0).GetChild(0);
            
            leftShoulder.localRotation = Quaternion.Euler(leftShoulder.localRotation.x - 90f, leftShoulder.localRotation.y, leftShoulder.localRotation.z);
                
            leftHand.localScale *= 0.8f;
            rightHand.localScale *= 0.8f;
            
        }
        
    }
}
