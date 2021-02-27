using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DiscordNotifier
{
    public enum ValheimEvent
    {
        OnServerStarted,
        OnServerStopped,
        OnPlayerJoined,
        OnPlayerDisconnected,
        OnPlayerDeath,
        OnPlayerMessage,
    }

    public class ValheimEventHandler
    {
        private static bool IsTrackingAll() => Main.zNet.IsServer() || Main.configTrackAllUsers.Value;
        private static bool IsTrackingUserId(ZDOID zdoId) => !Main.zNet.IsServer() && !Main.configTrackAllUsers.Value && Player.m_localPlayer.GetZDOID().userID == zdoId.userID;

        public static void OnServerStarted(string ipAddress = null)
        {
            if (!Main.configEvents[ValheimEvent.OnServerStarted].Value) return;

            if (ipAddress == null)
            {
                Utils.PostMessage("Server has started");
            }
            else
            {
                Utils.PostMessage("Server has started at: " + ipAddress);
            }
        }


        public static void OnServerStopped()
        {
            if (!Main.configEvents[ValheimEvent.OnServerStopped].Value) return;

            Utils.PostMessage("Server has stopped");
        }


        public static void OnPlayerJoined(ZNet.PlayerInfo playerInfo)
        {
            if (!Main.configEvents[ValheimEvent.OnPlayerJoined].Value) return;
            if (playerInfo.m_characterID.IsNone() || !IsTrackingAll() && !IsTrackingUserId(playerInfo.m_characterID)) return;

            Main.StaticLogger.LogInfo($"Player joined: {playerInfo.m_name} ({playerInfo.m_characterID})");
            Utils.PostMessage($"{playerInfo.m_name} has joined the server!");
        }

        public static void OnPlayerDisconnected(ZNet.PlayerInfo playerInfo)
        {
            if (!Main.configEvents[ValheimEvent.OnPlayerDisconnected].Value) return;
            if (playerInfo.m_characterID.IsNone() || !IsTrackingAll() && !IsTrackingUserId(playerInfo.m_characterID)) return;

            Main.StaticLogger.LogInfo($"Player left: {playerInfo.m_name} ({playerInfo.m_characterID})");
            Utils.PostMessage($"{playerInfo.m_name} has left the server!");
        }

        public static void OnPlayerDeath(ZNet.PlayerInfo playerInfo)
        {
            if (!Main.configEvents[ValheimEvent.OnPlayerDeath].Value) return;
            if (playerInfo.m_characterID.IsNone() || !IsTrackingAll() && !IsTrackingUserId(playerInfo.m_characterID)) return;

            Main.StaticLogger.LogInfo($"Player died: {playerInfo.m_name} ({playerInfo.m_characterID})");
            Utils.PostMessage($"{playerInfo.m_name} has died!");
        }
        public static void OnPlayerMessage(Talker.Type type, string user, string message, Vector3 pos)
        {
            if (!Main.configEvents[ValheimEvent.OnPlayerMessage].Value) return;

            switch (type)
            {
                case Talker.Type.Whisper:
                    break;
                case Talker.Type.Normal:
                    Utils.PostMessage(message, $"{user} said");
                    break;
                case Talker.Type.Shout:
                    Utils.PostMessage(message.ToUpper(), $"{user} yelled");
                    break;
                case Talker.Type.Ping:
                    Utils.PostMessage($"Location: ({pos.x}x, {pos.y}y, {pos.z}z)", $"{user} pinged");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }


        }
    }
}