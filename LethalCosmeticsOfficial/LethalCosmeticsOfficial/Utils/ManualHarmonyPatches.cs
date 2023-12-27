using System;
using HarmonyLib;

namespace LethalCosmeticsOfficial
{
    public class ManualHarmonyPatches
    {
        public static void ManualPatch(Harmony HarmonyInstance)
        {
            HarmonyInstance.Patch(AccessTools.Method(typeof(HUDManager), "SyncAllPlayerLevelsServerRpc", new Type[0]), new HarmonyMethod(typeof(HUDManagerPatch).GetMethod("ManualPrefix")));
        }
    }
}
