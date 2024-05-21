using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RehabDB;
using ReadyPlayerMe.Core;

namespace Game.Bicycle
{
public class AvatarOnBike : MonoBehaviour
{
    private string avatarURL;
    private GameObject bike;
    private GameObject avatar;
    // Start is called before the first frame update
    void Start()
    {
        // avatarURL = "https://models.readyplayer.me/65253564f3a2574b36a5ec18.glb";
        if (DBManager.Instance.currentPatient != null) avatarURL = DBManager.Instance.currentPatient.Avatar;
        else avatarURL = "https://models.readyplayer.me/65253564f3a2574b36a5ec18.glb";
        
        // Create bike
        bike = GameObject.Find("GhostBike");
        // Load avatar
        var avatarLoader = new AvatarObjectLoader();

        // Define the OnCompleted callback
        //avatarLoader.OnCompleted += OnAvatarLoaded;
        avatarLoader.OnCompleted += (_, args) =>
        {
            avatar = args.Avatar; // Assign the loaded avatar to the 'avatar' variable.
            AvatarAnimationHelper.SetupAnimator(args.Metadata, args.Avatar); // set the metadata for the avatar

            // Setup avatar correctly
            SetupAvatar(avatar);
        };

        // Load the avatar
        avatarLoader.LoadAvatar(avatarURL);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetupAvatar(GameObject avatar)
    {
        // Connect avatar to bike and fix position
        avatar.transform.SetParent(bike.transform);
        avatar.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
        avatar.transform.localPosition = new Vector3(0f, -0.6f, -0.5f);

        // Adjust angles of avatar joints
        Transform spine = avatar.transform.Find("Armature/Hips/Spine");
        Transform head = avatar.transform.Find("Armature/Hips/Spine/Spine1/Spine2/Neck/Head");
        Transform leftArm = avatar.transform.Find("Armature/Hips/Spine/Spine1/Spine2/LeftShoulder/LeftArm");
        Transform leftForeArm = avatar.transform.Find("Armature/Hips/Spine/Spine1/Spine2/LeftShoulder/LeftArm/LeftForeArm");
        Transform rightArm = avatar.transform.Find("Armature/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm");
        Transform rightForeArm = avatar.transform.Find("Armature/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm");
        Transform leftHand = avatar.transform.Find("Armature/Hips/Spine/Spine1/Spine2/LeftShoulder/LeftArm/LeftForeArm/LeftHand");
        Transform rightHand = avatar.transform.Find("Armature/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand");
        Transform leftUpLeg = avatar.transform.Find("Armature/Hips/LeftUpLeg");
        Transform rightUpLeg = avatar.transform.Find("Armature/Hips/RightUpLeg");
        Transform leftLeg = avatar.transform.Find("Armature/Hips/LeftUpLeg/LeftLeg");
        Transform rightLeg = avatar.transform.Find("Armature/Hips/RightUpLeg/RightLeg");
        if (spine != null && head != null && leftArm != null && rightArm != null && leftHand != null && rightHand != null && leftUpLeg != null && rightUpLeg != null && leftLeg != null && rightLeg != null)
        {
            // Adjust the local rotation of the LeftUpLeg
            spine.localRotation = Quaternion.Euler(40f, 0f, 0f);
            head.localRotation = Quaternion.Euler(-48f, 0f, 0f);
            leftArm.localRotation = Quaternion.Euler(60f, 80f, 0f);
            leftForeArm.localRotation = Quaternion.Euler(12f, -25f, -30f);
            rightArm.localRotation = Quaternion.Euler(60f, -80f, 0f);
            rightForeArm.localRotation = Quaternion.Euler(12f, 25f, 30f);
            leftHand.localRotation = Quaternion.Euler(-13f, -50f, 10f);
            rightHand.localRotation = Quaternion.Euler(-13f, 50f, -10f);
            leftUpLeg.localRotation = Quaternion.Euler(-45f, 0f, 175f);
            rightUpLeg.localRotation = Quaternion.Euler(-45f, 0f, -175f);
            leftLeg.localRotation = Quaternion.Euler(-75f, -20f, 15f);
            rightLeg.localRotation = Quaternion.Euler(-75f, 20f, -15);
        }
        else
        {
            if (spine == null) Debug.LogError("Spine not found in the avatar's hierarchy.");
            if (head == null) Debug.LogError("Head not found in the avatar's hierarchy.");
            if (leftArm == null) Debug.LogError("LeftArm not found in the avatar's hierarchy.");
            if (leftForeArm == null) Debug.LogError("LeftForeArm not found in the avatar's hierarchy.");
            if (rightArm == null) Debug.LogError("RightArm not found in the avatar's hierarchy.");
            if (rightForeArm == null) Debug.LogError("RightForeArm not found in the avatar's hierarchy.");
            if (leftHand == null) Debug.LogError("LeftHand not found in the avatar's hierarchy.");
            if (rightHand == null) Debug.LogError("RightHand not found in the avatar's hierarchy.");
            if (leftUpLeg == null) Debug.LogError("LeftUpLeg not found in the avatar's hierarchy.");
            if (rightUpLeg == null) Debug.LogError("RightUpLeg not found in the avatar's hierarchy.");
            if (leftLeg == null) Debug.LogError("LeftLeg not found in the avatar's hierarchy.");
            if (rightLeg == null) Debug.LogError("RightLeg not found in the avatar's hierarchy.");
        }
    }
}
}