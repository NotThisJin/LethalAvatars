using GameNetcodeStuff;
using HarmonyLib;
using LethalCosmeticsOfficial.Avatar;
using LethalCosmeticsOfficial.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace LethalCosmeticsOfficial.Patches
{
    [HarmonyPatch(typeof(HUDManager), "AddTextToChatOnServer")]
    public static class SendChatToServerPatch
    {
        public static bool Prefix(HUDManager __instance, string chatMessage, int playerId = -1)
        {
            if (chatMessage.StartsWith("[lethalavatar]"))
            {
                ReflectionUtils.InvokeMethod(__instance, "AddPlayerChatMessageServerRpc", new object[] { chatMessage, 99 });
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(HUDManager), "AddPlayerChatMessageServerRpc")]
    public static class ServerReceiveMessagePatch
    {
        public static string previousDataMessage = "";

        public static bool Prefix(HUDManager __instance, ref string chatMessage, int playerId)
        {
            NetworkManager networkManager = __instance.NetworkManager;
            if (networkManager == null || !networkManager.IsListening)
            {
                return false;
            }

            if (chatMessage.StartsWith("[lethalavatar]") && networkManager.IsServer)
            {
                previousDataMessage = chatMessage;
                chatMessage = "[replacewithavatardata]";
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
    public static class ConnectClientToPlayerObjectPatch
    {
        public static void Postfix(PlayerControllerB __instance)
        {
            string built = "[lethalavatar]";
            built += ";" + __instance.playerClientId;
            built += ";" + AvatarRegistry.localSelectedAvatar;
            HUDManager.Instance.AddTextToChatOnServer(built);
        }
    }

    [HarmonyPatch(typeof(HUDManager), "AddChatMessage")]
    public static class AddChatMessagePatch
    {
        public static bool Prefix(HUDManager __instance, string chatMessage, string nameOfUserWhoTyped = "")
        {
            if (chatMessage.StartsWith("[replacewithavatardata]") || chatMessage.StartsWith("[lethalavatar]"))
            {
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(HUDManager), "AddPlayerChatMessageClientRpc")]
    public static class ClientReceiveMessagePatch
    {
        public static bool ignoreSample = false;

        public static bool Prefix(HUDManager __instance, ref string chatMessage, int playerId)
        {
            NetworkManager networkManager = __instance.NetworkManager;
            if (networkManager == null || !networkManager.IsListening)
            {
                return false;
            }

            if (networkManager.IsServer)
            {
                if (chatMessage.StartsWith("[replacewithavatardata]"))
                {
                    chatMessage = ServerReceiveMessagePatch.previousDataMessage;
                    HandleDataMessage(chatMessage);
                }
                else if (chatMessage.StartsWith("[lethalavatar]"))
                {
                    // The server already handled this when the server branch was dealing with it.
                    return false;
                }
            }
            else
            {
                if (chatMessage.StartsWith("[lethalavatar]"))
                {
                    HandleDataMessage(chatMessage);
                }
            }

            return true;
        }
        private static void HandleDataMessage(string chatMessage)
        {
            if (ignoreSample)
            {
                return;
            }
            chatMessage = chatMessage.Replace("[lethalavatar]", "");
            string[] splitMessage = chatMessage.Split(';');
            string playerIdString = splitMessage[1];

            int playerIdNumeric = int.Parse(playerIdString);

            AvatarApplication existingAvatarApplication = StartOfRound.Instance.allPlayerScripts[playerIdNumeric].transform.Find("ScavengerModel")
                .Find("metarig").gameObject.GetComponent<AvatarApplication>();

            if (existingAvatarApplication)
            {
                existingAvatarApplication.ClearAvatar();
                GameObject.Destroy(existingAvatarApplication);
            }

            AvatarApplication avatarApplication = StartOfRound.Instance.allPlayerScripts[playerIdNumeric].transform.Find("ScavengerModel")
                .Find("metarig").gameObject.AddComponent<AvatarApplication>();

            avatarApplication.ClearAvatar();

            List<string> avatarsToApply = new List<string>();

            foreach (var cosmeticId in splitMessage)
            {
                if (cosmeticId == playerIdString)
                {
                    continue;
                }
                avatarsToApply.Add(cosmeticId);

                if (LethalCosmeticsOfficialBase.showAvatars)
                {
                    avatarApplication.ApplyAvatar(cosmeticId, true);
                }
            }

            if (playerIdNumeric == StartOfRound.Instance.thisClientPlayerId)
            {
                avatarApplication.ClearAvatar();
            }

            avatarApplication.spawnedAvatar.transform.localScale *= 0.38f;

            LethalCosmeticsOfficialBase.playerIdsAndAvatar.Remove(playerIdNumeric);
            LethalCosmeticsOfficialBase.playerIdsAndAvatar.Add(playerIdNumeric, avatarsToApply);

            if (GameNetworkManager.Instance.isHostingGame && (playerIdNumeric != 0))
            {
                ignoreSample = true;
                foreach (var keyPair in LethalCosmeticsOfficialBase.playerIdsAndAvatar)
                {
                    string built = "[lethalavatar]";
                    built += ";" + keyPair.Key;
                    foreach (var cosmetic in keyPair.Value)
                    {
                        built += ";" + cosmetic;
                    }
                    HUDManager.Instance.AddTextToChatOnServer(built);
                }

                ignoreSample = false;
            }
        }
    }
}