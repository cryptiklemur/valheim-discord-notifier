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
        public static void OnServerStarted(string ipAddress = null)
        {
            if (!Main.Configuration.Events[ValheimEvent.OnServerStarted].Value) return;

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
            if (!Main.Configuration.Events[ValheimEvent.OnServerStopped].Value) return;

            Utils.PostMessage("Server has stopped");
        }


        public static void OnPlayerJoined(ZNet.PlayerInfo playerInfo)
        {
            if (!Main.Configuration.Events[ValheimEvent.OnPlayerJoined].Value || playerInfo.m_characterID.IsNone()) return;

            Utils.PostMessage(
                Main.Configuration.EventMessages[ValheimEvent.OnPlayerJoined].Value
                    .Replace("{{username}}", playerInfo.m_name)
                    .Replace("{{userId}}", playerInfo.m_characterID.userID.ToString())
            );
        }

        public static void OnPlayerDisconnected(ZNet.PlayerInfo playerInfo)
        {
            if (!Main.Configuration.Events[ValheimEvent.OnPlayerDisconnected].Value || playerInfo.m_characterID.IsNone()) return;

            Utils.PostMessage(
                Main.Configuration.EventMessages[ValheimEvent.OnPlayerDisconnected].Value
                    .Replace("{{username}}", playerInfo.m_name)
                    .Replace("{{userId}}", playerInfo.m_characterID.userID.ToString())
            );
        }

        public static void OnPlayerDeath(ZNet.PlayerInfo playerInfo)
        {
            if (!Main.Configuration.Events[ValheimEvent.OnPlayerDeath].Value || playerInfo.m_characterID.IsNone()) return;

            Utils.PostMessage(
                Main.Configuration.EventMessages[ValheimEvent.OnPlayerDeath].Value
                    .Replace("{{username}}", playerInfo.m_name)
                    .Replace("{{userId}}", playerInfo.m_characterID.userID.ToString())
            );
        }
        public static void OnPlayerMessage(Talker.Type type, string user, string message, Vector3 pos)
        {
            if (!Main.Configuration.Events[ValheimEvent.OnPlayerMessage].Value) return;

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