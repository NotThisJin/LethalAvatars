using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Newtonsoft;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LethalCosmeticsOfficial
{
    [BepInPlugin(MODGUID, MODNAME, MODVERSION)]
    public class LethalCosmeticsOfficialBase : BaseUnityPlugin
    {
        public const string MODGUID = "NotThisJin.LethalAvatars";
        public const string MODNAME = "LethalAvatars";
        public const string MODVERSION = "1.0.0";

        private readonly Harmony harmony = new Harmony(MODGUID);
        public static LethalCosmeticsOfficialBase Instance;

        public static bool showAvatars = true;

        public static LocalCosmeticJson localCosmetic;

        public static Dictionary<int, List<string>> playerIdsAndAvatar = new Dictionary<int, List<string>>();

        public static string cosmeticSavePath = Application.persistentDataPath + "/lethalCosmetics.json";

        public static ManualLogSource logger;

        void Awake()
        {
            if (Instance == null) 
                Instance = this;

            logger = Logger;

            Logger.LogInfo($"Lethal Avatars have been loaded!");

            try
            {
                harmony.PatchAll();
            }
            catch( Exception e )
            {
                Logger.LogInfo("Failed to patch: " + e);
            }

            ManualHarmonyPatches.ManualPatch(harmony);
            CheckCreateJson();
        }

        public void CheckCreateJson()
        {
            if (!File.Exists(cosmeticSavePath))
            {
                localCosmetic = new LocalCosmeticJson();
                File.WriteAllText(cosmeticSavePath, JsonConvert.SerializeObject(localCosmetic, Formatting.Indented));
                Logger.LogInfo("Local Cosmetic Json Created!");
            }
            else
            {
                localCosmetic = JsonConvert.DeserializeObject<LocalCosmeticJson>(File.ReadAllText(cosmeticSavePath));
            }
        }

        public void UpdateCosmeticJson()
        {
            if (File.Exists(cosmeticSavePath))
            {

            }
            else
            {
                CheckCreateJson();
                Logger.LogError("Cosmetic Json Doesn't Exist!");
            }
        }
    }

    public static class HUDManagerPatch
    {
        public static bool ManualPrefix(HUDManager __instance)
        {
            return false;
        }
    }

}
