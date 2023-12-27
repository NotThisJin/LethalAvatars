using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LethalCosmeticsOfficial.Avatar
{
    public class AvatarInstance : MonoBehaviour
    {
        public string avatarId;
        public Texture2D icon;

        public List<DynamicBones> DynamicBones;
        public List<ColliderBones> ColliderBones;
    }

    [Serializable]
    public class ColliderBones
    {
        [HideInInspector]
        public string fontName;

        public Transform Bone;

        public enum Direction
        {
            X, Y, Z
        }

        public Direction m_Direction = Direction.Y;

        public Vector3 m_Center = Vector3.zero;

        public enum Bound
        {
            Outside,
            Inside
        }
        public Bound m_Bound = Bound.Outside;

        public float m_Radius = 0.5f;

        public float m_Height = 0;
    }

    [Serializable]
    public class DynamicBones
    {
        [HideInInspector]
        public string fontName;

        public Transform m_Root = null;

        public float m_UpdateRate = 60.0f;

        public enum UpdateMode
        {
            Normal,
            AnimatePhysics,
            UnscaledTime
        }
        public UpdateMode m_UpdateMode = UpdateMode.Normal;

        [Range(0, 1)]
        public float m_Damping = 0.1f;
        public AnimationCurve m_DampingDistrib = null;

        [Range(0, 1)]
        public float m_Elasticity = 0.1f;
        public AnimationCurve m_ElasticityDistrib = null;

        [Range(0, 1)]
        public float m_Stiffness = 0.1f;
        public AnimationCurve m_StiffnessDistrib = null;

        [Range(0, 1)]
        public float m_Inert = 0;
        public AnimationCurve m_InertDistrib = null;

        public float m_Radius = 0;
        public AnimationCurve m_RadiusDistrib = null;

        public float m_EndLength = 0;

        public Vector3 m_EndOffset = Vector3.zero;

        public Vector3 m_Gravity = Vector3.zero;

        public Vector3 m_Force = Vector3.zero;

        public List<Transform> m_Colliders = null;

        public List<Transform> m_Exclusions = null;

        public enum FreezeAxis
        {
            None, X, Y, Z
        }

        public FreezeAxis m_FreezeAxis = FreezeAxis.None;

        public bool m_DistantDisable = false;
        public Transform m_ReferenceObject = null;
        public float m_DistanceToObject = 20;

        Vector3 m_LocalGravity = Vector3.zero;
        Vector3 m_ObjectMove = Vector3.zero;
        Vector3 m_ObjectPrevPosition = Vector3.zero;
        float m_BoneTotalLength = 0;
        float m_ObjectScale = 1.0f;
        float m_Time = 0;
        float m_Weight = 1.0f;
        bool m_DistantDisabled = false;

        class Particle
        {
            public Transform m_Transform = null;
            public int m_ParentIndex = -1;
            public float m_Damping = 0;
            public float m_Elasticity = 0;
            public float m_Stiffness = 0;
            public float m_Inert = 0;
            public float m_Radius = 0;
            public float m_BoneLength = 0;

            public Vector3 m_Position = Vector3.zero;
            public Vector3 m_PrevPosition = Vector3.zero;
            public Vector3 m_EndOffset = Vector3.zero;
            public Vector3 m_InitLocalPosition = Vector3.zero;
            public Quaternion m_InitLocalRotation = Quaternion.identity;
        }

        List<Particle> m_Particles = new List<Particle>();
    }

}
