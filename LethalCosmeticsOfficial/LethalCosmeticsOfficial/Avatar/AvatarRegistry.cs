using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using LethalCosmeticsOfficial.Utils;

namespace LethalCosmeticsOfficial.Avatar
{
    public class AvatarRegistry
    {
        public static Dictionary<string, AvatarInstance> avatarInstances = new Dictionary<string, AvatarInstance>();

        public static GameObject avatarGUI;
        private static GameObject displayGuy;
        private static AvatarApplication avatarApplication;
        public static string localSelectedAvatar;

        public static void LoadCosmeticsFromBundle(AssetBundle bundle)
        {
            foreach (var potentialPrefab in bundle.GetAllAssetNames())
            {
                if (!potentialPrefab.EndsWith(".prefab"))
                {
                    continue;
                }

                GameObject cosmeticInstance = bundle.LoadPersistentAsset<GameObject>(potentialPrefab);
                AvatarInstance cosmeticInstanceBehavior = cosmeticInstance.GetComponent<AvatarInstance>();
                if (cosmeticInstanceBehavior == null)
                {
                    continue;
                }
                LethalCosmeticsOfficialBase.logger.LogInfo("Loaded cosmetic: " + cosmeticInstanceBehavior.avatarId + " from bundle");
                avatarInstances.Add(cosmeticInstanceBehavior.avatarId, cosmeticInstanceBehavior);
            }
        }
    }
}
