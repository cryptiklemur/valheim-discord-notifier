using System;
using System.Collections.Generic;
using UnityEngine;

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
        OnPlayerShout,
        OnPlayerWhisper,
        OnPlayerPing,
    }

    public class ValheimEventHandler
    {
        private static string GetRandomMessage(IReadOnlyList<string> messages) => messages[new System.Random().Next(0, messages.Count)];

        public static void OnServerStarted(string ipAddress = null)
        {
            if (!Main.Configuration.Events[ValheimEvent.OnServerStarted].Value) return;

            Utils.PostMessage(GetRandomMessage(Main.Configuration.messages.OnServerStart).Replace("{{serverAddress}}", ipAddress));
        }


        public static void OnServerStopped()
        {
            if (!Main.Configuration.Events[ValheimEvent.OnServerStopped].Value) return;

            Utils.PostMessage(GetRandomMessage(Main.Configuration.messages.OnServerStop));
        }


        public static void OnPlayerJoined(ZNet.PlayerInfo playerInfo)
        {
            if (!Main.Configuration.Events[ValheimEvent.OnPlayerJoined].Value || playerInfo.m_characterID.IsNone()) return;


            Utils.PostMessage(
                GetRandomMessage(Main.Configuration.messages.OnPlayerJoined)
                    .Replace("{{username}}", playerInfo.m_name)
                    .Replace("{{userId}}", playerInfo.m_characterID.userID.ToString())
            );
        }

        public static void OnPlayerDisconnected(ZNet.PlayerInfo playerInfo)
        {
            if (!Main.Configuration.Events[ValheimEvent.OnPlayerDisconnected].Value || playerInfo.m_characterID.IsNone()) return;

            Utils.PostMessage(
                GetRandomMessage(Main.Configuration.messages.OnPlayerDisconnect)
                    .Replace("{{username}}", playerInfo.m_name)
                    .Replace("{{userId}}", playerInfo.m_characterID.userID.ToString())
            );
        }

        public static void OnPlayerDeath(ZNet.PlayerInfo playerInfo)
        {
            if (!Main.Configuration.Events[ValheimEvent.OnPlayerDeath].Value || playerInfo.m_characterID.IsNone()) return;

            Utils.PostMessage(
                GetRandomMessage(Main.Configuration.messages.OnPlayerDeath)
                    .Replace("{{username}}", playerInfo.m_name)
                    .Replace("{{userId}}", playerInfo.m_characterID.userID.ToString())
            );
        }
        public static void OnPlayerMessage(Talker.Type type, string user, string message, Vector3 pos)
        {

            switch (type)
            {
                case Talker.Type.Whisper:
                    if (!Main.Configuration.Events[ValheimEvent.OnPlayerWhisper].Value) return;

                    Utils.PostMessage(message, $"{user} said");
                    break;
                case Talker.Type.Normal:
                    if (!Main.Configuration.Events[ValheimEvent.OnPlayerMessage].Value) return;

                    Utils.PostMessage(message, $"{user} said");
                    break;
                case Talker.Type.Shout:
                    if (!Main.Configuration.Events[ValheimEvent.OnPlayerShout].Value) return;
                    if (Main.Configuration.UpperCaseShout.Value) message = message.ToUpper();

                    Utils.PostMessage(message, $"{user} yelled");
                    break;
                case Talker.Type.Ping:
                    if (!Main.Configuration.Events[ValheimEvent.OnPlayerPing].Value) return;

                    Utils.PostMessage($"Location: ({pos.x}x, {pos.y}y, {pos.z}z)", $"{user} pinged");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}