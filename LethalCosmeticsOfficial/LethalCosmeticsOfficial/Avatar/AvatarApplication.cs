using LethalCosmeticsOfficial.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;
using static UnityEngine.GraphicsBuffer;

namespace LethalCosmeticsOfficial.Avatar
{
    public class AvatarApplication : MonoBehaviour
    {

        public BoneMapping bones;

        public AvatarInstance spawnedAvatar;

        GameObject LOD1;
        GameObject LOD2;
        GameObject LOD3;

        private GameObject newHip;

        public void Awake()
        {
            bones.baseTransform = transform.parent;

            // Body Bones
            bones.hip = transform.Find("spine");
            bones.spine = bones.hip.Find("spine.001");
            bones.chest = bones.spine.Find("spine.002");
            bones.upperChest = bones.chest.Find("spine.003");
            bones.head = bones.upperChest.Find("spine.004");

            // Left Arm
            bones.shoulderL = bones.upperChest.Find("shoulder.L");
            bones.upperArmLeft = bones.shoulderL.Find("arm.L_upper");
            bones.lowerArmLeft = bones.upperArmLeft.Find("arm.L_lower");
            bones.leftHand = bones.lowerArmLeft.Find("hand.L");

            // Right Arm
            bones.shoulderR = bones.upperChest.Find("shoulder.R");
            bones.upperArmRight = bones.shoulderR.Find("arm.R_upper");
            bones.lowerArmRight = bones.upperArmRight.Find("arm.R_lower");
            bones.rightHand = bones.lowerArmRight.Find("hand.R");

            // Left Leg
            bones.upperLegLeft = bones.hip.Find("thigh.L");
            bones.lowerLegLeft = bones.upperLegLeft.Find("shin.L");
            bones.footLeft = bones.lowerLegLeft.Find("foot.L");

            // Right Leg
            bones.upperLegRight = bones.hip.Find("thigh.R");
            bones.lowerLegRight = bones.upperLegLeft.Find("shin.R");
            bones.footRight = bones.lowerLegLeft.Find("foot.R");

            // Left Hand
            bones.Thumb1L = bones.leftHand.Find("finger1.L");
            bones.Thumb2L = bones.Thumb1L.Find("finger1.L.001");

            bones.Index1L = bones.leftHand.Find("finger2.L");
            bones.Index2L = bones.Index1L.Find("finger2.L.001");

            bones.Mid1L = bones.leftHand.Find("finger3.L");
            bones.Mid2L = bones.Mid1L.Find("finger3.L.001");

            bones.Ring1L = bones.leftHand.Find("finger4.L");
            bones.Ring2L = bones.Ring1L.Find("finger4.L.001");

            bones.Pinky1L = bones.leftHand.Find("finger5.L");
            bones.Pinky2L = bones.Pinky1L.Find("finger5.L.001");

            // Right Hand
            bones.Thumb1R = bones.leftHand.Find("finger1.R");
            bones.Thumb2R = bones.Thumb1R.Find("finger1.R.001");

            bones.Index1R = bones.leftHand.Find("finger2.R");
            bones.Index2R = bones.Index1R.Find("finger2.R.001");

            bones.Mid1R = bones.leftHand.Find("finger3.R");
            bones.Mid2R = bones.Mid1R.Find("finger3.R.001");

            bones.Ring1R = bones.leftHand.Find("finger4.R");
            bones.Ring2R = bones.Ring1R.Find("finger4.R.001");

            bones.Pinky1R = bones.leftHand.Find("finger5.R");
            bones.Pinky2R = bones.Pinky1R.Find("finger5.R.001");

            // LOD Meshes
            LOD1 = bones.baseTransform.Find("LOD1").gameObject;
            LOD2 = bones.baseTransform.Find("LOD2").gameObject;
            LOD3 = bones.baseTransform.Find("LOD3").gameObject;
        }

        private void OnDisable()
        {
            spawnedAvatar.gameObject.SetActive(false);
            ToggleLOD(true);
        }

        private void OnEnable()
        {
            spawnedAvatar.gameObject.SetActive(true);
            ToggleLOD(false);
        }

        public void ClearAvatar()
        {
            GameObject.Destroy(spawnedAvatar.gameObject);
            GameObject.Destroy(newHip);
            spawnedAvatar = null;
        }

        public void ToggleLOD(bool toggle)
        {
            LOD1.SetActive(toggle);
            LOD2.SetActive(toggle);
            LOD3.SetActive(toggle);
        }

        public void ApplyAvatar(string avatarId,bool startEnabled)
        {
            if(AvatarRegistry.avatarInstances.ContainsKey(avatarId))
            {
                AvatarInstance avatarInstance = AvatarRegistry.avatarInstances[avatarId];
                GameObject avatarInstanceGameObject = GameObject.Instantiate(avatarInstance.gameObject);
                avatarInstanceGameObject.SetActive(startEnabled);
                AvatarInstance avatarInstanceBehaviour = avatarInstanceGameObject.GetComponent<AvatarInstance>();
                spawnedAvatar = avatarInstanceBehaviour;
                if (startEnabled)
                {
                    ConnectAvatar(avatarInstanceBehaviour);
                }
            }
        }

        public void RefreshAvatar()
        {
            ConnectAvatar(spawnedAvatar);
        }

        public void ConnectAvatar(AvatarInstance avatarInstance) 
        {
            Animator animator = avatarInstance.GetComponent<Animator>();
            UnityEngine.Avatar avatar = animator.avatar;

            ToggleLOD(false);

            if (avatar != null)
            {
                HumanBodyBones[] allBones = (HumanBodyBones[])Enum.GetValues(typeof(HumanBodyBones));

                foreach(HumanBodyBones bone in allBones)
                {
                    if (animator.GetBoneTransform(bone) == null)
                        continue;

                    switch (bone)
                    {
                        case HumanBodyBones.Hips:
                            animator.GetBoneTransform(bone).position = bones.hip.position - new Vector3(0, 0.35f, 0);
                            animator.GetBoneTransform(bone).rotation = bones.hip.rotation;
                            animator.GetBoneTransform(bone).parent = bones.hip;
                            newHip = animator.GetBoneTransform(bone).gameObject;
                            break;
                        case HumanBodyBones.Spine:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.spine);
                            break;
                        case HumanBodyBones.Chest:

                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.chest);
                            break;
                        case HumanBodyBones.UpperChest:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.upperChest);
                            break;
                        case HumanBodyBones.Head:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.head);
                            break;
                        case HumanBodyBones.LeftShoulder:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.shoulderL);
                            break;
                        case HumanBodyBones.RightShoulder:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.shoulderR);
                            break;
                        case HumanBodyBones.LeftUpperArm:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.upperArmLeft);
                            break;
                        case HumanBodyBones.RightUpperArm:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.upperArmRight);
                            break;
                        case HumanBodyBones.LeftLowerArm:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.lowerArmLeft);
                            break;
                        case HumanBodyBones.RightLowerArm:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.lowerArmRight);
                            break;
                        case HumanBodyBones.LeftHand:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.leftHand);
                            break;
                        case HumanBodyBones.RightHand:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.rightHand);
                            break;
                        case HumanBodyBones.LeftUpperLeg:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.upperLegLeft);
                            break;
                        case HumanBodyBones.RightUpperLeg:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.upperLegRight);
                            break;
                        case HumanBodyBones.LeftLowerLeg:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.lowerLegLeft);
                            break;
                        case HumanBodyBones.RightLowerLeg:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.lowerLegRight);
                            break;
                        case HumanBodyBones.LeftFoot:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.footLeft);
                            break;
                        case HumanBodyBones.RightFoot:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.footRight);
                            break;
                        case HumanBodyBones.LeftThumbProximal:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.Thumb1L);
                            break;
                        case HumanBodyBones.LeftThumbIntermediate:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.Thumb2L);
                            break;
                        case HumanBodyBones.LeftIndexProximal:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.Index1L);
                            break;
                        case HumanBodyBones.LeftIndexIntermediate:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.Index2L);
                            break;
                        case HumanBodyBones.LeftMiddleProximal:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.Mid1L);
                            break;
                        case HumanBodyBones.LeftMiddleIntermediate:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.Mid2L);
                            break;
                        case HumanBodyBones.LeftRingProximal:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.Ring1L);
                            break;
                        case HumanBodyBones.LeftRingIntermediate:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.Ring2L);
                            break;
                        case HumanBodyBones.LeftLittleProximal:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.Pinky1L);
                            break;
                        case HumanBodyBones.LeftLittleIntermediate:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.Pinky2L);
                            break;
                        case HumanBodyBones.RightThumbProximal:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.Thumb1R);
                            break;
                        case HumanBodyBones.RightThumbIntermediate:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.Thumb2R);
                            break;
                        case HumanBodyBones.RightIndexProximal:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.Index1R);
                            break;
                        case HumanBodyBones.RightIndexIntermediate:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.Index2R);
                            break;
                        case HumanBodyBones.RightMiddleProximal:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.Mid1R);
                            break;
                        case HumanBodyBones.RightMiddleIntermediate:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.Mid2R);
                            break;
                        case HumanBodyBones.RightRingProximal:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.Ring1R);
                            break;
                        case HumanBodyBones.RightRingIntermediate:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.Ring2R);
                            break;
                        case HumanBodyBones.RightLittleProximal:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.Pinky1R);
                            break;
                        case HumanBodyBones.RightLittleIntermediate:
                            AddRotationConstraintComponent(animator.GetBoneTransform(bone), bones.Pinky2R);
                            break;
                    }
                }
            }

            //// Body Bones
            //if(animator.GetBoneTransform(HumanBodyBones.Hips) != null)
            //{
            //    animator.GetBoneTransform(HumanBodyBones.Hips).position = bones.hip.position - new Vector3(0, 0.35f, 0);
            //    animator.GetBoneTransform(HumanBodyBones.Hips).rotation = bones.hip.rotation;
            //    animator.GetBoneTransform(HumanBodyBones.Hips).parent = bones.hip;
            //    newHip = animator.GetBoneTransform(HumanBodyBones.Hips).gameObject;
            //}
            //if(animator.GetBoneTransform(HumanBodyBones.Spine) != null)
            //{
            //    AddRotationConstraintComponent(animator.GetBoneTransform(HumanBodyBones.Spine), bones.spine);
            //}
            //if (animator.GetBoneTransform(HumanBodyBones.Chest) != null)
            //{
            //    AddRotationConstraintComponent(animator.GetBoneTransform(HumanBodyBones.Chest), bones.chest);
            //}
            //if (animator.GetBoneTransform(HumanBodyBones.UpperChest) != null)
            //{
            //    AddRotationConstraintComponent(animator.GetBoneTransform(HumanBodyBones.UpperChest), bones.upperChest);
            //}
            //if (animator.GetBoneTransform(HumanBodyBones.Spine) != null)
            //{
            //    AddRotationConstraintComponent(animator.GetBoneTransform(HumanBodyBones.Spine), bones.spine);
            //}
        }

        void AddRotationConstraintComponent(Transform targetObject, Transform sourceObject)
        {
            if (targetObject.GetComponent<RotationConstraint>() == null)
            {
                RotationConstraint rotationConstraint = targetObject.gameObject.AddComponent<RotationConstraint>();
                ConstraintSource constraintSource = new ConstraintSource
                {
                    sourceTransform = sourceObject.transform,
                    weight = 1f
                };
                rotationConstraint.AddSource(constraintSource);

                rotationConstraint.locked = false;

                rotationConstraint.rotationOffset = Vector3.zero;

                rotationConstraint.locked = true;
                rotationConstraint.constraintActive = true;
                LethalCosmeticsOfficialBase.logger.LogWarning($"Added Rotation Constraint for {targetObject}.");
            }
            else
            {
                //LethalCosmeticsOfficialBase.logger.LogError("Rotation Constraint component already exists on the GameObject.");
            }
        }
    }
}
